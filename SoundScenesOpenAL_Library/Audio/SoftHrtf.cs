using System;
using OpenTK.Audio.OpenAL;

namespace SoundScenesOpenAL_Library.Audio
{
    // Tryb HRTF wybierany przy tworzeniu kontekstu OpenAL
    public enum HrtfMode
    {
        Default,   // brak atrybutu - OpenAL Soft sam decyduje (np. włącza HRTF, gdy wykryje słuchawki)
        Enabled,   // wymuszenie HRTF
        Disabled   // wymuszenie braku HRTF
    }

    // Faktyczny stan HRTF zgłaszany przez OpenAL Soft (wartości ALC_HRTF_*_SOFT z alext.h)
    public enum HrtfStatus
    {
        Disabled = 0x0000,           // ALC_HRTF_DISABLED_SOFT - wyłączony
        Enabled = 0x0001,            // ALC_HRTF_ENABLED_SOFT - włączony
        Denied = 0x0002,             // ALC_HRTF_DENIED_SOFT - wyłączony, bo niedozwolony na urządzeniu
        Required = 0x0003,           // ALC_HRTF_REQUIRED_SOFT - włączony, bo urządzenie/konfiguracja go wymaga
        HeadphonesDetected = 0x0004, // ALC_HRTF_HEADPHONES_DETECTED_SOFT - włączony automatycznie (słuchawki)
        UnsupportedFormat = 0x0005   // ALC_HRTF_UNSUPPORTED_FORMAT_SOFT - wyłączony, bo wyjście nie jest stereo
    }

    // Obsługa rozszerzenia ALC_SOFT_HRTF z OpenAL Soft. OpenTK 4.9.4 nie ma dla niego nazw,
    // więc stałe są przepisane z oficjalnych źródeł:
    //  - nagłówek: https://github.com/kcat/openal-soft/blob/1.24.3/include/AL/alext.h#L440-L453
    //  - specyfikacja: https://openal-soft.org/openal-extensions/SOFT_HRTF.txt
    public static class SoftHrtf
    {
        public const string ExtensionName = "ALC_SOFT_HRTF";

        // #define ALC_HRTF_SOFT 0x1992 - atrybut kontekstu: żądanie włączenia/wyłączenia HRTF
        private const int AlcHrtfSoft = 0x1992;

        // #define ALC_HRTF_STATUS_SOFT 0x1993 - parametr alcGetIntegerv: faktyczny stan HRTF
        private const int AlcHrtfStatusSoft = 0x1993;

        // alc.h: #define ALC_TRUE 1, #define ALC_FALSE 0
        private const int AlcTrue = 1;
        private const int AlcFalse = 0;

        // Lista atrybutów dla alcCreateContext musi kończyć się zerem
        private const int AttributeListEnd = 0;

        // Atrybuty dla ALC.CreateContext; null = zachowanie domyślne (jak przed dodaniem HRTF)
        public static int[]? BuildContextAttributes(ALDevice device, HrtfMode mode)
        {
            if (mode == HrtfMode.Default)
                return null;

            if (!ALC.IsExtensionPresent(device, ExtensionName))
                throw new InvalidOperationException($"Urządzenie OpenAL nie obsługuje rozszerzenia {ExtensionName}.");

            int value = mode == HrtfMode.Enabled ? AlcTrue : AlcFalse;
            return new[] { AlcHrtfSoft, value, AttributeListEnd };
        }

        // Faktyczny stan HRTF po utworzeniu kontekstu; null, gdy rozszerzenie jest niedostępne.
        // Nieznana wartość (spec dopuszcza nowe w przyszłości) zostaje jako liczba w enumie.
        public static HrtfStatus? ReadStatus(ALDevice device)
        {
            if (!ALC.IsExtensionPresent(device, ExtensionName))
                return null;

            return (HrtfStatus)ALC.GetInteger(device, (AlcGetInteger)AlcHrtfStatusSoft);
        }
    }
}
