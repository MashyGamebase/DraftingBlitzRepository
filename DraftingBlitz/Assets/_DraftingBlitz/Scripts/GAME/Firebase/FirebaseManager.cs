using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;

public class FirebaseManager : Singleton<FirebaseManager>
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference dbReference;

    [Header("UI Elements")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    [Header("")]
    public string userNickname;

    void Start()
    {
        // Initialize Firebase
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference; // Initialize Database

        DontDestroyOnLoad(gameObject);
    }

    public async void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (!IsValidEmail(email))
        {
            Debug.LogError("Invalid email format.");
            PopupController.Instance.PopupNotif("Invalid email format.", 2f);
            return;
        }

        try
        {
            // Register user
            var userCredential = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = userCredential.User;
            Debug.Log($"User registered: {user.Email}");

            // Send verification email
            await user.SendEmailVerificationAsync();
            Debug.Log("Verification email sent.");
            PopupController.Instance.PopupNotif("Verification email sent. Please verify your email before logging in.", 2f);

            // Store user in Firebase Database
            await SaveUserData(user);
        }
        catch (Exception e)
        {
            Debug.LogError($"Registration error: {e.Message}");
            PopupController.Instance.PopupNotif($"Registration failed: {e.Message}", 2f);
        }
    }

    public async Task SaveUserData(FirebaseUser newUser)
    {
        string userId = newUser.UserId;
        UserData userData = new UserData(newUser.Email, DateTime.UtcNow.ToString(), userNickname);

        string json = JsonUtility.ToJson(userData);

        try
        {
            await dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
            Debug.Log("User data saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save user data: {e.Message}");
        }
    }

    public async void SaveUserDataProxy()
    {
        string userId = user.UserId;
        UserData userData = new UserData(user.Email, DateTime.UtcNow.ToString(), userNickname);

        string json = JsonUtility.ToJson(userData);

        try
        {
            await dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
            Debug.Log("User data saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save user data: {e.Message}");
        }
    }

    public async void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        try
        {
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = userCredential.User;

            if (!user.IsEmailVerified)
            {
                Debug.LogWarning("Email not verified.");
                PopupController.Instance.PopupNotif("Email not verified. Please check your email and verify your account.", 2f);
                auth.SignOut();
                return;
            }

            Debug.Log($"User logged in: {user.Email}");

            // Fetch the user's nickname from Firebase
            await FetchUserNickname(user.UserId);

            FadeController.Instance.StartFakeLoading("LoadingScreen", true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Login error: {e.Message}");
            PopupController.Instance.PopupNotif($"Login failed: {e.Message}", 2f);
        }
    }

    public async Task FetchUserNickname(string userId)
    {
        try
        {
            var snapshot = await dbReference.Child("users").Child(userId).Child("nickname").GetValueAsync();
            if (snapshot.Exists)
            {
                userNickname = snapshot.Value.ToString();
                Debug.Log($"Fetched nickname: {userNickname}");
            }
            else
            {
                Debug.LogWarning("Nickname not found.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching nickname: {e.Message}");
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public void LogoutUser()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            PopupController.Instance.PopupNotif("User logged out.", 2f);
            FadeController.Instance?.StartFakeLoading("MainMenu", false);
            Debug.Log("User logged out.");
        }
    }
}

// Data Model for Firebase
[Serializable]
public class UserData
{
    public string email;
    public string registeredAt;
    public string nickname;

    public UserData(string email, string registeredAt, string nickname)
    {
        this.email = email;
        this.registeredAt = registeredAt;
        this.nickname = nickname;
    }
}
