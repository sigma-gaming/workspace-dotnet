namespace Core.Services.Gaming.Runners;

/// <summary>
///     Калькулятор для игры в кости, который вычисляет множители и суммы выигрышей
///     на основе выбранных игроком сторон кубика и размера ставки.
/// </summary>
public static class DiceCalculator
{
    private const decimal RTP = 0.95m;
    private const int DICE_SIDES = 6;

    /// <summary>
    ///     Вычисляет множитель выигрыша на основе выбранных сторон кубика.
    ///     Множитель зависит от количества выбранных сторон и RTP.
    /// </summary>
    /// <param name="sides">Набор выбранных сторон кубика</param>
    /// <returns>Множитель для расчета выигрыша</returns>
    public static decimal CalculateMultiplier(IReadOnlySet<int> sides)
    {
        return DICE_SIDES / (decimal)sides.Count * RTP;
    }

    /// <summary>
    ///     Вычисляет чистую сумму выигрыша (без учета ставки) для заданной комбинации.
    /// </summary>
    /// <param name="bet">Сумма ставки</param>
    /// <param name="sides">Выбранные стороны кубика</param>
    /// <param name="multiplier">Опциональный предвычисленный множитель</param>
    /// <returns>Сумма чистого выигрыша (без ставки)</returns>
    public static long CalculateWinAmount(
        long bet,
        IReadOnlySet<int> sides,
        decimal? multiplier = null)
    {
        var actualMultiplier = multiplier ?? CalculateMultiplier(sides);
        return (long)decimal.Floor(bet * actualMultiplier - bet);
    }
}