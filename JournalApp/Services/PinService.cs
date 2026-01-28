using Microsoft.Maui.Storage;

namespace JournalApp.Services
{
    public class PinService
    {
        private const string PIN_KEY = "journal_pin";
        private const string IS_SETUP_KEY = "pin_setup";
        private const string FIXED_PIN = "1234";

        public async Task<bool> IsPinSetupAsync()
        {
            try
            {
                var isSetup = await SecureStorage.GetAsync(IS_SETUP_KEY);
                if (!string.IsNullOrEmpty(isSetup))
                {
                    return true;
                }
                
                // Fallback to Preferences
                return Preferences.ContainsKey(IS_SETUP_KEY);
            }
            catch
            {
                // Fallback to Preferences
                return Preferences.ContainsKey(IS_SETUP_KEY);
            }
        }

        public async Task<bool> ValidatePinAsync(string pin)
        {
            try
            {
                var storedPin = await SecureStorage.GetAsync(PIN_KEY);
                if (!string.IsNullOrEmpty(storedPin))
                {
                    return storedPin == pin;
                }
                
                // Fallback to Preferences
                var prefPin = Preferences.Get(PIN_KEY, "");
                return prefPin == pin;
            }
            catch
            {
                // Fallback to Preferences
                var prefPin = Preferences.Get(PIN_KEY, "");
                return prefPin == pin;
            }
        }

        public async Task<bool> SetupPinAsync(string pin)
        {
            try
            {
                // Allow any 4-digit PIN for initial setup
                if (string.IsNullOrEmpty(pin) || pin.Length != 4 || !pin.All(char.IsDigit))
                {
                    return false;
                }

                // Try SecureStorage first
                try
                {
                    await SecureStorage.SetAsync(PIN_KEY, pin);
                    await SecureStorage.SetAsync(IS_SETUP_KEY, "true");
                    return true;
                }
                catch (Exception ex)
                {
                    // Fallback to Preferences if SecureStorage fails
                    System.Diagnostics.Debug.WriteLine($"SecureStorage failed, using Preferences: {ex.Message}");
                    Preferences.Set(PIN_KEY, pin);
                    Preferences.Set(IS_SETUP_KEY, "true");
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"PIN Setup Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePinAsync(string currentPin, string newPin)
        {
            try
            {
                var storedPin = await SecureStorage.GetAsync(PIN_KEY);
                
                // Validate current PIN
                if (storedPin != currentPin)
                {
                    return false;
                }

                // Validate new PIN
                if (string.IsNullOrEmpty(newPin) || newPin.Length != 4 || !newPin.All(char.IsDigit))
                {
                    return false;
                }

                // Update to new PIN
                await SecureStorage.SetAsync(PIN_KEY, newPin);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetFixedPin()
        {
            return FIXED_PIN;
        }
    }
}
