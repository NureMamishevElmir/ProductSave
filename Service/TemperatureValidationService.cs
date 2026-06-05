namespace Service;

public sealed class TemperatureValidationService
{
    // Минимальная температура (абсолютный ноль в Цельсиях)
    private const double MinTemperature = -273.15;
    
    // Максимальная температура (разумный предел для хранения продуктов)
    private const double MaxTemperature = 100.0;

    /// <summary>
    /// Валидирует значение температуры
    /// </summary>
    /// <param name="temperature">Значение температуры для валидации</param>
    /// <param name="errorMessage">Сообщение об ошибке, если валидация не прошла</param>
    /// <returns>true, если значение валидно, иначе false</returns>
    public bool ValidateTemperature(double? temperature, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (temperature == null)
        {
            errorMessage = "Температура не может быть null.";
            return false;
        }

        if (temperature < MinTemperature)
        {
            errorMessage = $"Температура не может быть ниже абсолютного нуля ({MinTemperature}°C).";
            return false;
        }

        if (temperature > MaxTemperature)
        {
            errorMessage = $"Температура не может превышать {MaxTemperature}°C.";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Валидирует значение температуры (перегрузка для nullable double)
    /// </summary>
    /// <param name="temperature">Значение температуры для валидации</param>
    /// <param name="errorMessage">Сообщение об ошибке, если валидация не прошла</param>
    /// <param name="allowNull">Разрешить null значения (по умолчанию false)</param>
    /// <returns>true, если значение валидно, иначе false</returns>
    public bool ValidateTemperature(double? temperature, out string errorMessage, bool allowNull)
    {
        errorMessage = string.Empty;

        if (temperature == null)
        {
            if (allowNull)
            {
                return true;
            }
            errorMessage = "Температура не может быть null.";
            return false;
        }

        return ValidateTemperature(temperature, out errorMessage);
    }
}
