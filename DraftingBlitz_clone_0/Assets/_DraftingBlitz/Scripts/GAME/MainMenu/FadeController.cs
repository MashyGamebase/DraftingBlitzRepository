using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeController : Singleton<FadeController>
{
    public Animator animator;
    public GameObject loadingText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartFakeLoading(string sceneToLoad, bool isInstantLoad = false)
    {
        StartCoroutine(fakeLoadingCO(sceneToLoad, isInstantLoad));
    }

    IEnumerator fakeLoadingCO(string sceneToLoad, bool isInstantLoad = false)
    {
        animator.SetTrigger("FadeIn");

        if (!isInstantLoad)
        {
            yield return new WaitForSeconds(1.5f);

            //loadingText.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            SceneManager.LoadSceneAsync(sceneToLoad);

            yield return new WaitForSeconds(3f);

            //loadingText.gameObject.SetActive(false);

            yield return new WaitForSeconds(2f);
        }
        else
        {
            yield return new WaitForSeconds(0.8f);

            SceneManager.LoadScene(sceneToLoad);
        }

        animator.SetTrigger("FadeOut");
    }
}