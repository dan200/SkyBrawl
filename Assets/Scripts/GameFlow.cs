using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public RawImage HUD;
    public RawImage ControlsScreen;
    public RawImage GameOverScreen;
    public RawImage ContinueScreen;

    public GameObject KillIconPrefab;
    public GameObject DeathIconPrefab;

    [Serializable]
    public struct MedalIconData
    {
        public GameObject Prefab;
        public int KillThreshold;
    }
    public List<MedalIconData> MedalIcons;

    public PlayerJetControl PlayerJet;

    public AudioSource IconStampSound;

    public InputAction ContinueInput;

    private enum State
    {
        Controls,
        InGame,
        GameOver
    }
    private State m_state;

    private struct ScoreboardData
    {
        public int NumKills;
        public bool Survived;
    }
    ScoreboardData m_scoreboardData;
    private bool m_hasEverLeftRunway;

    private int m_remainingScoreboardIcons;
    private float m_scoreboardIconTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_state = State.Controls;
        ContinueInput.Enable();
        GameOverScreen.enabled = false;
        ContinueScreen.enabled = false;
    }

    bool CheckGameOver(out bool o_survived)
    {
        if(PlayerJet == null)
        {
            o_survived = false;
            return true;
        }

        bool onRunway = PlayerJet.GetComponent<JetCollision>().IsOnRunway;
        bool stopped = (PlayerJet.GetComponent<JetPhysics>().Speed < 4.0f) && (PlayerJet.GetComponent<Rigidbody>().angularVelocity.magnitude * Mathf.Rad2Deg < 10.0f);
        bool engineWorking = PlayerJet.GetComponent<JetEngine>().EngineWorking;
        if(onRunway && stopped && (m_hasEverLeftRunway || !engineWorking))
        {
            o_survived = true;
            return true;
        }

        o_survived = false;
        return false;
    }

    public GameObject PickMedalPrefab(int kills)
    {
        for(int i=MedalIcons.Count-1; i>=0; --i)
        {
            var data = MedalIcons[i];
            if(kills >= data.KillThreshold)
            {
                return data.Prefab;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        switch(m_state)
        {
            case State.Controls:
                {
                    if(ContinueInput.triggered)
                    {
                        ControlsScreen.enabled = false;
                        PlayerJet.enabled = true;

                        m_state = State.InGame;
                        m_remainingScoreboardIcons = 1;
                        ContinueInput.Disable();
                    }
                    break;
                }
            case State.InGame:
                {
                    if(PlayerJet != null)
                    {
                        m_scoreboardData.NumKills = PlayerJet.GetComponent<KillTracker>().KillCount;
                        if(!PlayerJet.GetComponent<JetCollision>().IsOnRunway)
                        {
                            m_hasEverLeftRunway = true;
                        }
                    }

                    bool survived;
                    if (CheckGameOver(out survived))
                    {
                        m_scoreboardData.Survived = survived;

                        HUD.enabled = false;
                        GameOverScreen.enabled = true;
                        if(PlayerJet != null)
                        {
                            PlayerJet.enabled = false;
                            PlayerJet.GetComponent<JetEngine>().EngineTurnedOn = false;
                        }

                        m_state = State.GameOver;
                        m_remainingScoreboardIcons = 1 + m_scoreboardData.NumKills;
                        m_scoreboardIconTimer = 3.0f;
                    }
                    break;
                }
            case State.GameOver:
                {
                    if(m_remainingScoreboardIcons > 0)
                    {
                        m_scoreboardIconTimer -= dt;
                        if(m_scoreboardIconTimer <= 0.0f)
                        {
                            if(m_remainingScoreboardIcons == 1)
                            {
                                if(m_scoreboardData.Survived)
                                {
                                    Instantiate(PickMedalPrefab(m_scoreboardData.NumKills), GameOverScreen.transform);
                                }
                                else
                                {
                                    Instantiate(DeathIconPrefab, GameOverScreen.transform);
                                }
                            }
                            else
                            {
                                Instantiate(KillIconPrefab, GameOverScreen.transform);
                            }
                            m_remainingScoreboardIcons--;
                            m_scoreboardIconTimer = 0.5f;

                            if(IconStampSound != null)
                            {
                                IconStampSound.Play();
                            }

                            if (m_remainingScoreboardIcons == 0)
                            {
                                ContinueScreen.enabled = true;
                                ContinueInput.Enable();
                            }
                        }
                    }

                    if(ContinueInput.enabled && ContinueInput.triggered)
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
                    }
                    break;
                }
        }
    }
}
