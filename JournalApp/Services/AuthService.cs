using Microsoft.Maui.Storage;

namespace JournalApp.Services
{
    public class AuthService
    {
        private const string AUTH_KEY = "is_authenticated";

        public event Action? OnAuthenticationChanged;

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var isAuthenticated = await SecureStorage.GetAsync(AUTH_KEY);
                if (!string.IsNullOrEmpty(isAuthenticated))
                {
                    return isAuthenticated == "true";
                }
                
                // Fallback to Preferences
                return Preferences.Get(AUTH_KEY, false);
            }
            catch
            {
                // Fallback to Preferences
                return Preferences.Get(AUTH_KEY, false);
            }
        }

        public async Task SetAuthenticatedAsync(bool authenticated)
        {
            try
            {
                await SecureStorage.SetAsync(AUTH_KEY, authenticated ? "true" : "false");
                OnAuthenticationChanged?.Invoke();
            }
            catch (Exception ex)
            {
                // Fallback to Preferences if SecureStorage fails
                System.Diagnostics.Debug.WriteLine($"SecureStorage failed for auth, using Preferences: {ex.Message}");
                Preferences.Set(AUTH_KEY, authenticated);
                OnAuthenticationChanged?.Invoke();
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await SecureStorage.SetAsync(AUTH_KEY, "false");
                OnAuthenticationChanged?.Invoke();
            }
            catch (Exception ex)
            {
                // Fallback to Preferences if SecureStorage fails
                System.Diagnostics.Debug.WriteLine($"SecureStorage failed for logout, using Preferences: {ex.Message}");
                Preferences.Set(AUTH_KEY, false);
                OnAuthenticationChanged?.Invoke();
            }
        }
    }
}
