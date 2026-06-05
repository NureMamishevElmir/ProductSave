# ProductSave — масштабування бекенда (Kubernetes)

Інструкція з розгортання API у Kubernetes-кластері та проведення
навантажувального тестування для лабораторної роботи №4.

Стек: .NET 8 Web API + PostgreSQL, горизонтальне масштабування подів,
`LoadBalancer` Service як балансувальник, HPA для автомасштабування,
навантажувальне тестування — k6.

## 0. Передумови

- **Docker Desktop** зі **ввімкненим Kubernetes**
  (`Settings → Kubernetes → Enable Kubernetes → Apply & Restart`).
- `kubectl` (постачається з Docker Desktop).
- Контекст kubectl вказує на Docker Desktop:
  ```powershell
  kubectl config use-context docker-desktop
  kubectl get nodes        # має показати вузол docker-desktop у статусі Ready
  ```
- **k6** для навантажувального тесту (`winget install k6 --source winget`
  або `choco install k6`). Альтернативно — через Docker (див. §6).

## 1. Збірка образу API

Docker Desktop ділить демон Docker з Kubernetes, тому локально зібраний образ
одразу доступний кластеру (з `imagePullPolicy: IfNotPresent`, без реєстру).

```powershell
# з кореня репозиторію
docker build -t productsave-api:latest .
```

## 2. Розгортання

Маніфести пронумеровані за порядком застосування. Один рядок піднімає все:

```powershell
kubectl apply -f deploy/k8s
```

Що буде створено в namespace `productsave`:

| Файл | Об'єкт | Роль |
|------|--------|------|
| `00-namespace.yaml`    | Namespace | ізоляція ресурсів |
| `10-secrets.yaml`      | Secret ×2 | credentials БД, рядок підключення, JWT |
| `20-postgres.yaml`     | PVC + Deployment + Service | БД зі сталим сховищем |
| `30-api-deployment.yaml` | Deployment | репліки API (stateless) |
| `40-api-service.yaml`  | Service (LoadBalancer) | балансувальник навантаження |
| `50-api-hpa.yaml`      | HorizontalPodAutoscaler | автомасштабування за CPU |

Перевірити стан:

```powershell
kubectl -n productsave get pods,svc,hpa
kubectl -n productsave rollout status deployment/api
```

API стане доступним за адресою **http://localhost/** (Swagger:
http://localhost/swagger), бо на Docker Desktop `LoadBalancer` публікується
на localhost.

Швидка перевірка:

```powershell
curl http://localhost/api/Products
curl http://localhost/health/ready
```

## 3. Демонстрація ручного масштабування

Збільшити/зменшити кількість подів і побачити, що система працює коректно:

```powershell
kubectl -n productsave scale deployment/api --replicas=5
kubectl -n productsave get pods -w        # спостерігати появу нових подів
# ... трафік далі обслуговується; балансувальник підхоплює нові поди
kubectl -n productsave scale deployment/api --replicas=2
```

Балансування можна побачити, якщо повертати ім'я пода у відповіді або
дивитися логи кількох подів одночасно:

```powershell
kubectl -n productsave logs -l app=api --prefix -f
```

## 4. Автомасштабування (HPA)

HPA потребує **metrics-server**, якого немає в Docker Desktop за замовчуванням.
Встановити (з прапорцем для самопідписаних сертифікатів kubelet):

```powershell
kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml
kubectl -n kube-system patch deployment metrics-server --type=json `
  -p '[{"op":"add","path":"/spec/template/spec/containers/0/args/-","value":"--kubelet-insecure-tls"}]'
```

Перевірити, що метрики надходять, і спостерігати за HPA під навантаженням:

```powershell
kubectl top pods -n productsave
kubectl -n productsave get hpa api -w
```

Під час навантаження (§5) HPA підніматиме репліки 2 → … → 10, коли середнє
CPU перевищить 60% від запиту (`requests.cpu = 200m`), і поверне назад після
завершення тесту.

## 5. Навантажувальне тестування (автоматичний експеримент)

Скрипт послідовно фіксує кількість реплік, чекає готовності подів, запускає
k6 і зберігає JSON-зведення для кожного прогону:

```powershell
./loadtest/run-scaling-experiment.ps1 -Replicas 1,2,4,8 -BaseUrl http://localhost
```

Наприкінці він друкує таблицю `replicas → rps → p95`, а файли
`loadtest/results/summary-rN.json` використовуються для побудови графіка
«кількість реплік ↔ витримуваний RPS» у звіті.

## 6. Навантажувальне тестування (один прогін вручну)

```powershell
# k6 встановлено локально
k6 run --tag replicas=2 loadtest/k6/products-load.js

# або через Docker (без локального k6); host.docker.internal = localhost хоста
docker run --rm -i -e BASE_URL=http://host.docker.internal `
  grafana/k6 run - < loadtest/k6/products-load.js
```

## 7. Прибирання

```powershell
kubectl delete namespace productsave
# metrics-server (за потреби):
kubectl delete -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml
```

## Структура

```
deploy/
  k8s/                     маніфести Kubernetes (застосовуються за порядком номерів)
  README.md                цей файл
loadtest/
  k6/products-load.js      сценарій k6 (GET /api/Products, ramping VUs)
  run-scaling-experiment.ps1  автоматичний прогін для різної кількості реплік
  results/                 JSON-зведення прогонів (створюється скриптом)
```
