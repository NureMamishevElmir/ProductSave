// k6 load test for the ProductSave API.
//
// Target: GET /api/Products — a public, read-only, DB-backed endpoint. It is
// the cleanest way to measure raw throughput because it needs no auth token
// yet still exercises the full path: load balancer -> API pod -> PostgreSQL.
//
// Goal of the lab: run this against the cluster with a fixed number of API
// replicas, record the sustained requests/sec and latency, then change the
// replica count and re-run. The relationship between replicas and throughput
// is the core result of the report.
//
// Usage (k6 binary installed locally):
//   k6 run loadtest/k6/products-load.js
//   BASE_URL=http://localhost k6 run loadtest/k6/products-load.js
//
// Usage (via Docker, no local install — reach the host from the container):
//   docker run --rm -i -e BASE_URL=http://host.docker.internal `
//     grafana/k6 run - < loadtest/k6/products-load.js
//
// Tag a run so results are easy to compare between replica counts:
//   k6 run --tag replicas=2 loadtest/k6/products-load.js

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend, Rate } from 'k6/metrics';

const BASE_URL = __ENV.BASE_URL || 'http://localhost';
const ENDPOINT = `${BASE_URL}/api/Products`;

// Custom metrics so the summary clearly separates this endpoint.
const productsLatency = new Trend('products_latency', true);
const productsFailRate = new Rate('products_failed');

// Peak concurrency and stage durations are overridable via env so the same
// script serves both a quick live run and a full report run. Defaults below
// are the "full" profile.
const PEAK_VUS = parseInt(__ENV.PEAK_VUS || '400', 10);
const WARM = __ENV.WARM || '30s';
const PUSH = __ENV.PUSH || '1m';
const HOLD = __ENV.HOLD || '1m';
// Per-iteration think time. Set SLEEP=0 to saturate the server (open-throttle
// closed model) and reveal the CPU/DB bottleneck — that is the scaling demo.
const SLEEP = parseFloat(__ENV.SLEEP || '0.2');

export const options = {
  // Ramping VUs: climb the concurrency until the API saturates, hold, then
  // ramp down. Throughput (http_reqs rate) plateaus once a resource is maxed
  // out — that plateau is the "withstood RPS" for the current replica count.
  scenarios: {
    ramp: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: WARM, target: Math.round(PEAK_VUS / 4) }, // warm up
        { duration: PUSH, target: PEAK_VUS },                 // push to peak
        { duration: HOLD, target: PEAK_VUS },                 // hold at peak
        { duration: '10s', target: 0 },                       // ramp down
      ],
      gracefulStop: '10s',
    },
  },
  thresholds: {
    // The system is "withstanding" the load if 95% of requests stay under
    // 800 ms and the error rate is below 1%.
    http_req_duration: ['p(95)<800'],
    products_failed: ['rate<0.01'],
  },
};

export default function () {
  const res = http.get(ENDPOINT);

  const ok = check(res, {
    'status is 200': (r) => r.status === 200,
  });

  productsLatency.add(res.timings.duration);
  productsFailRate.add(!ok);

  // Think-time so VUs model users rather than a tight hammer loop.
  // Set SLEEP=0 for a saturation test that exposes the server's capacity.
  if (SLEEP > 0) {
    sleep(SLEEP);
  }
}
