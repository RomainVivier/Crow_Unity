using UnityEngine;
using System.Collections;

public class CardPopup : MonoBehaviour
{
    #region attributes
    private const float TIMER_DURATION=1f;
    private Timer m_cardTimer;
    private Material m_currentMaterial;
    private Material m_targetMaterial;
    private Animator m_animator;
    private MeshRenderer m_meshRenderer;
    #endregion

    #region methods
    void Start ()
    {
        GadgetManager.Instance.CardPopup = this;
        m_animator = gameObject.GetComponent<Animator>();
        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        m_cardTimer=new Timer();
	}
	
	void Update ()
    {
        if(m_currentMaterial!=m_targetMaterial)
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).nameHash == -1783470761)//Animator.StringToHash("Down"))
                popupNow();
        }
        if (m_cardTimer.IsElapsedOnce) m_animator.SetBool("isUp", false);
    }

    public void popup(Material newMaterial)
    {
        m_targetMaterial = newMaterial;
        if (m_animator.GetCurrentAnimatorStateInfo(0).nameHash == -1783470761)//Animator.StringToHash("Down"))
            popupNow();
        else m_animator.SetBool("isUp", false);
    }

    private void popupNow()
    {
        m_currentMaterial = m_targetMaterial;
        m_animator.SetBool("isUp", true);
        m_cardTimer.Reset(TIMER_DURATION);
        m_meshRenderer.material = m_currentMaterial;
    }
    #endregion
}
