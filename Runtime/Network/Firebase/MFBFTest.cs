#if USE_FIREBASE
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class MFBFTest : MonoBehaviour
    {
        [SerializeField]
        Text _info = null;

        string _userid;

        const string KEY = "users";
        const string SORT_KEY = "rank";

        public void OnLoginAnonymously()
        {
            MFBFNetwork.SignInAnonymously(userId =>
            {
                _info.text = $"{userId}がログインしました";
                _userid = userId;
            });
        }

        public void OnLoginAnalytics()
        {
            if (string.IsNullOrEmpty(_userid))
            {
                _info.text = "userIdを設定するためにログインしてください";
                return;
            }
            
            _info.text = "ログインしたことを通知します";
            MFBFNetwork.SetupAnalytics(_userid);
        }
        
        public class User {
            public string username;
            public string email;
            public int rank;

            public User(string username, string email, int rank) {
                this.username = username;
                this.email = email;
                this.rank = rank;
            }
        }
        
        public void OnGetById()
        {
            MFBFNetwork.GetById(KEY, _userid, s => { }, () => { });
        }
        
        public void OnGetAll()
        {
            MFBFNetwork.GetAll(KEY, s => { }, () => { });
        }
        
        public void OnGetOrderByFirst()
        {
            MFBFNetwork.GetOrderByFirst(KEY, SORT_KEY,2, null, null);
        }
        
        public void OnGetOrderByLast()
        {
            MFBFNetwork.GetOrderByLast(KEY, SORT_KEY,2, null, null);
        }

        public void OnSetById()
        {
            User kajiya = new User("k-r", "gmail", 1);
            string json = JsonUtility.ToJson(kajiya);
            MFBFNetwork.SetById(KEY, _userid, json, null, null);
            
            User hayato = new User("hayato", "ymail", 2);
            json = JsonUtility.ToJson(hayato);
            MFBFNetwork.SetById(KEY, "dadusadhasuod", json, null, null);
            
            User shouhei = new User("shouhei", "smail", 3);
            json = JsonUtility.ToJson(shouhei);
            MFBFNetwork.SetById(KEY, "dashoduasdao", json, null, null);
        }
        
    }
}

#endif
#endif