using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralUI : MonoBehaviour
{
    [SerializeField] private GameObject scrollablePanel;
    private GameObject originalRoot;
    public void ToggleUIVisible()
    {
        CanvasGroup cg = scrollablePanel.GetComponentInChildren<CanvasGroup>();
        cg.interactable= !cg.interactable;
        cg.alpha = cg.alpha == 0 ? 1 : 0;
        GameObject.FindObjectOfType<Finalization>().camCanMove = !cg.interactable;
        Camera.main.transform.rotation = Quaternion.identity;
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

    private void Start()
    {
        originalRoot = Instantiate(GameObject.FindWithTag("Root"), Vector3.zero, Quaternion.identity);
        originalRoot.SetActive(false);
    }

    public void DeleteAllFacade()
    {
        // removes every constructed object, reverts back to OG
        var newRoot = GameObject.FindWithTag("Root");
        Destroy(newRoot);
        originalRoot.SetActive(true);
        Instantiate(originalRoot, Vector3.zero, Quaternion.identity);
        originalRoot.SetActive(false);
        GameObject.FindObjectOfType<Notification>().SetNotice("Everything deleted!");
        GameObject.FindObjectOfType<SelectionHandler>().SelectRoot();
        GameObject.FindObjectsOfType<CreateGrid>().ToList().ForEach(o => o.Start());
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
