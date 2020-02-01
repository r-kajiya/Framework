using System;

namespace Framework
{
    public static class Network
    {
        public static void Initialize(string userId, Action<string> onSignInRegister)
        {
#if USE_FIREBASE
            // FirebaseAuthentication.Initialize(userId, onSignInRegister);
#endif
        }
        
    }
}
