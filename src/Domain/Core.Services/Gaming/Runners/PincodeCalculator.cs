using Core.Contracts.Enums;

namespace Core.Services.Gaming.Runners;

/// <summary>
///     Калькулятор для игры Pincode, который обрабатывает различные комбинации чисел
///     и вычисляет соответствующие множители выигрыша.
/// </summary>
public static class PincodeCalculator
{
    private const int CODE_LENGTH = 4;

    // Основная конфигурация для всех режимов игры
    private static readonly Dictionary<PincodeMode, ModeConfig> Config = new()
    {
        // Конфигурация для легкого режима
        [PincodeMode.Easy] = new ModeConfig
        {
            CombinationMap = new Dictionary<int, string>
            {
                [0] = "0000",
                [1111] = "1111",
                [2222] = "2222",
                [3333] = "3333",
                [4444] = "4444",
                [5555] = "5555",
                [6666] = "6666",
                [7777] = "7777",
                [8888] = "8888",
                [9999] = "9999",
                [1337] = "1337",
                [1488] = "1488",
                [1234] = "1234",
                [4321] = "4321",
            },
            MultiplierMap = new Dictionary<string, int>
            {
                ["2x0"] = 2, // Две цифры 0
                ["2x9"] = 2, // Две цифры 9
                ["2x7"] = 5, // Две цифры 7
                ["3xN"] = 10, // Три одинаковые цифры
                ["3x7"] = 14, // Три цифры 7
                ["1111"] = 50, // Четыре единицы
                ["2222"] = 50, // Четыре двойки
                ["3333"] = 50, // Четыре тройки
                ["4444"] = 50, // Четыре четверки
                ["5555"] = 50, // Четыре пятерки
                ["8888"] = 50, // Четыре восьмерки
                ["6666"] = 66, // Четыре шестерки
                ["0000"] = 100, // Четыре нуля
                ["9999"] = 100, // Четыре девятки
                ["1234"] = 111, // Последовательность вперед
                ["4321"] = 111, // Последовательность назад
                ["1337"] = 137, // Специальная комбинация
                ["1488"] = 148, // Специальная комбинация
                ["7777"] = 345, // Четыре семерки
            },
            HighlightMap = new Dictionary<int, int>(), // Заполняется в конструкторе
        },

        // Конфигурация для сложного режима
        [PincodeMode.Hardcore] = new ModeConfig
        {
            CombinationMap = new Dictionary<int, string>
            {
                [0] = "0000",
                [1111] = "1111",
                [2222] = "2222",
                [3333] = "3333",
                [4444] = "4444",
                [5555] = "5555",
                [6666] = "6666",
                [7777] = "7777",
                [8888] = "8888",
                [9999] = "9999",
                [1337] = "1337",
                [1488] = "1488",
                [1234] = "1234",
                [4321] = "4321",
            },
            MultiplierMap = new Dictionary<string, int>
            {
                ["2x7"] = 7, // Две семерки
                ["3x7"] = 77, // Три семерки
                ["1111"] = 69, // Четыре единицы
                ["2222"] = 69, // Четыре двойки
                ["3333"] = 69, // Четыре тройки
                ["4444"] = 69, // Четыре четверки
                ["5555"] = 69, // Четыре пятерки
                ["8888"] = 69, // Четыре восьмерки
                ["0000"] = 100, // Четыре нуля
                ["9999"] = 100, // Четыре девятки
                ["1234"] = 123, // Последовательность вперед
                ["4321"] = 321, // Последовательность назад
                ["1337"] = 337, // Специальная комбинация
                ["1488"] = 488, // Специальная комбинация
                ["6666"] = 666, // Четыре шестерки
                ["7777"] = 777, // Четыре семерки
            },
            HighlightMap = new Dictionary<int, int>(), // Заполняется в конструкторе
        },
    };

    // Статический конструктор инициализирует все конфигурации при первом обращении к классу
    static PincodeCalculator()
    {
        InitializeConfigurations();
    }

    /// <summary>
    ///     Устанавливает бит в маске по указанному индексу.
    /// </summary>
    private static int WithBit(int bitmask, int index)
    {
        var bit = 1 << (CODE_LENGTH - 1 - index);
        return bitmask | bit;
    }

    /// <summary>
    ///     Проверяет наличие бита в маске по указанному индексу.
    /// </summary>
    private static bool HasBit(int bitmask, int index)
    {
        var bit = 1 << (CODE_LENGTH - 1 - index);
        return (bitmask & bit) != 0;
    }

    /// <summary>
    ///     Отмечает позиции указанного числа в коде битовой маской.
    /// </summary>
    private static int MarkNumbers(int bitmask, string codeString, int number)
    {
        var result = bitmask;
        for (var i = 0; i < CODE_LENGTH; i++)
            if (codeString[i] == number.ToString()[0])
                result = WithBit(result, i);

        return result;
    }

    /// <summary>
    ///     Устанавливает комбинацию для указанного кода, если она лучше существующей.
    /// </summary>
    private static void SetCombination(PincodeMode mode, int code, string combination)
    {
        var currentCombination = GetPincodeCombination(mode, code);
        if (currentCombination == null)
        {
            Config[mode].CombinationMap[code] = combination;
            return;
        }

        var oldMultiplier = Config[mode].MultiplierMap.GetValueOrDefault(currentCombination);
        var newMultiplier = Config[mode].MultiplierMap.GetValueOrDefault(combination);

        if (newMultiplier >= oldMultiplier) Config[mode].CombinationMap[code] = combination;
    }

    private static void InitializeConfigurations()
    {
        // Проходим по всем возможным четырехзначным кодам
        for (var code = 0; code < 10000; code++)
        {
            var codeString = code.ToString("D4");
            var countMap = new Dictionary<char, int>();

            // Подсчитываем количество каждой цифры в коде
            foreach (var digit in codeString) countMap[digit] = countMap.GetValueOrDefault(digit) + 1;

            // Проверяем различные комбинации для Easy режима
            if (countMap.GetValueOrDefault('0') == 2)
            {
                SetCombination(PincodeMode.Easy, code, "2x0");
                Config[PincodeMode.Easy].HighlightMap[code] = MarkNumbers(0, codeString, 0);
            }

            if (countMap.GetValueOrDefault('9') == 2)
            {
                SetCombination(PincodeMode.Easy, code, "2x9");
                Config[PincodeMode.Easy].HighlightMap[code] = MarkNumbers(0, codeString, 9);
            }

            // Проверяем комбинации для обоих режимов
            if (countMap.GetValueOrDefault('7') == 2)
            {
                SetCombination(PincodeMode.Easy, code, "2x7");
                SetCombination(PincodeMode.Hardcore, code, "2x7");
                Config[PincodeMode.Easy].HighlightMap[code] = MarkNumbers(0, codeString, 7);
                Config[PincodeMode.Hardcore].HighlightMap[code] = MarkNumbers(0, codeString, 7);
            }

            // Проверяем тройные комбинации для Easy режима
            foreach (var (digit, count) in countMap)
                if (count == 3)
                {
                    SetCombination(PincodeMode.Easy, code, "3xN");
                    Config[PincodeMode.Easy].HighlightMap[code] = MarkNumbers(0, codeString, digit - '0');
                }

            // Проверяем специальную комбинацию трех семерок для обоих режимов
            if (countMap.GetValueOrDefault('7') == 3)
            {
                SetCombination(PincodeMode.Easy, code, "3x7");
                SetCombination(PincodeMode.Hardcore, code, "3x7");
                Config[PincodeMode.Easy].HighlightMap[code] = MarkNumbers(0, codeString, 7);
                Config[PincodeMode.Hardcore].HighlightMap[code] = MarkNumbers(0, codeString, 7);
            }
        }
    }

    // Публичные методы для получения информации о комбинациях и множителях

    /// <summary>
    ///     Возвращает строковое представление комбинации для указанного кода и режима.
    /// </summary>
    private static string? GetPincodeCombination(PincodeMode mode, int code)
    {
        return Config[mode].CombinationMap.GetValueOrDefault(code);
    }

    /// <summary>
    ///     Возвращает множитель выигрыша для указанного кода и режима.
    /// </summary>
    public static int GetPincodeMultiplier(PincodeMode mode, int code)
    {
        var combination = GetPincodeCombination(mode, code);
        if (combination == null) return 0;
        return Config[mode].MultiplierMap.GetValueOrDefault(combination);
    }

    /// <summary>
    ///     Возвращает словарь всех возможных множителей для указанного режима.
    /// </summary>
    public static IReadOnlyDictionary<string, int> GetPincodeMultiplierMap(PincodeMode mode)
    {
        return Config[mode].MultiplierMap;
    }

    /// <summary>
    ///     Возвращает массив булевых значений, указывающих, какие позиции в коде должны быть подсвечены.
    /// </summary>
    public static bool[] GetPincodeHighlight(PincodeMode mode, int code)
    {
        var bitmask = Config[mode].HighlightMap.GetValueOrDefault(code);

        return
        [
            HasBit(bitmask, 0),
            HasBit(bitmask, 1),
            HasBit(bitmask, 2),
            HasBit(bitmask, 3),
        ];
    }

    private class ModeConfig
    {
        public Dictionary<int, string> CombinationMap { get; init; } = new();
        public Dictionary<string, int> MultiplierMap { get; init; } = new();
        public Dictionary<int, int> HighlightMap { get; init; } = new();
    }
}