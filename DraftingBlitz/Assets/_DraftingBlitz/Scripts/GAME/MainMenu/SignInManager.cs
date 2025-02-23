using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInManager : MonoBehaviour
{
    [Header("-- Buttons --")]
    public Button googleLoginButton;
    public Button facebookLoginButton;
    public Button guestLoginButton;

    // FadeController Reference
    FadeController fadeController => FadeController.Instance;

    private void Start()
    {
        googleLoginButton?.onClick.AddListener(Login_Google);
        facebookLoginButton?.onClick.AddListener(Login_Facebook);
        guestLoginButton?.onClick.AddListener(Login_Guest);
    }


    #region LOGIN_BUTTONS
    public void Login_Google()
    {
        GoogleLogin();
    }

    public void Login_Facebook()
    {
        FacebookLogin();
    }

    public void Login_Guest()
    {
        GuestLogin();
    }
    #endregion

    #region LOGIN_CALLBACKS
    private void GoogleLogin()
    {
        // Google Login Callback
    }

    private void FacebookLogin()
    {
        // Facebook Login Callback
    }

    private void GuestLogin()
    {
        // Guest Login Callback
        fadeController.StartFakeLoading("LoadingScreen", true);
    }
    #endregion
}