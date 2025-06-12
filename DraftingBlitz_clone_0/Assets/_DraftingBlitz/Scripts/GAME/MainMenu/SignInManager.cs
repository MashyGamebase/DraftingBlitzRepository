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
    [Space]
    public Button confirmLoginGoogleButton;
    public Button confirmRegisterGoogleButton;
    public Button confirmLoginFacebookButton;

    [Space]
    [Header("-- Other Buttons --")]
    public Button cancelGoogleLoginButton;
    public Button cancelFacebookLoginButton;

    // FadeController Reference
    FadeController fadeController => FadeController.Instance;

    // Main Menu Canvas Controller Reference
    MainMenuCanvasController mainMenuCanvasController => MainMenuCanvasController.Instance;

    private void Start()
    {
        AddButtons();
    }
    
    protected void AddButtons()
    {
        googleLoginButton?.onClick.AddListener(Login_Google);
        facebookLoginButton?.onClick.AddListener(Login_Facebook);
        guestLoginButton?.onClick.AddListener(Login_Guest);

        confirmLoginGoogleButton?.onClick.AddListener(ConfirmLoginGoogle);
        confirmRegisterGoogleButton?.onClick.AddListener(ConfirmRegisterGoogle);

        cancelGoogleLoginButton?.onClick.AddListener(CancelGoogleLogin);
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
        mainMenuCanvasController.ShowLoginRegisterPanel();
    }

    private void ConfirmLoginGoogle()
    {
        FirebaseManager.Instance.LoginUser();
    }

    private void ConfirmRegisterGoogle()
    {
        FirebaseManager.Instance.RegisterUser();
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

    private void CancelGoogleLogin()
    {
        // Google Login Callback
        mainMenuCanvasController.HideLoginRegisterPanel();
    }

    private void CancelFacebookLogin()
    {
        // Facebook Login Callback
    }
    #endregion
}