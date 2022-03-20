using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplaySpeech : MonoBehaviour
{
    //geting gameobjects
    public TMP_Text speech;
    public TMP_Text speaker;
    public GameObject speechBox;
    public GameObject speakerBox;
    public GameObject nextButton;
    public GameObject answerChoices;
    public GameObject charactersCanvas;
    public GameObject firstPath;
    public GameObject secondPath;
    public GameObject thirdPath;
    public GameObject[] characters;
    public GameObject finalPanels;
    public Image[] panels;
   
    void Awake()
    //initial conditions (intro: only backgroundcanvas and speechbox active)
    {
        speakerBox.SetActive(false);
        speechBox.SetActive(true);
        nextButton.SetActive(false);
        answerChoices.SetActive(false);
        charactersCanvas.SetActive(true);
        finalPanels.SetActive(false);
        for(int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
        }
    }
    void Start()
    {
        speaking = StartCoroutine(Speaking(narratorSpeech, 0));
    }

    void Update() 
    {
        if (!isSpeaking() && !isWaitingForUserInput)
        {
            if (nextSpeaker == "Herói")
            {
                speaking = StartCoroutine(Speaking(heroSpeech, heroIndex,  "Herói"));
                if (heroIndex < heroSpeech.Length)
                {
                    heroIndex++;
                }
            }
            else if (nextSpeaker == "Minotauro")
            {
                speaking = StartCoroutine(Speaking(minotaurSpeech, minotaurIndex,  "Minotauro"));
                if (minotaurIndex < minotaurSpeech.Length)
                {
                    minotaurIndex++;  
                }
            }
            if (minotaurIndex == minotaurSpeech.Length && heroIndex == heroSpeech.Length)
            {
                nextSpeaker ="Ninguém";

                if(!isSpeaking() && !isEnd)
                {
                    isWaitingForUserInput = true;
                    speechBox.SetActive(false);
                    speakerBox.SetActive(false);
                    SetAnswersButtons(true);
                    SetNextButton(false);
                }
                else if(!isSpeaking() && isEnd)
                {
                    if (choosenPath == 0)
                    {
                        characters[lastPoseIndex].SetActive(false);
                        characters[8].SetActive(true);
                        characters[9].SetActive(true);
                        speakerBox.SetActive(false);
                        speechBox.SetActive(false);
                        FindObjectOfType<AudioConfig> ().StopPlaying ("Background");

                        if(canPlay)
                        {
                            FindObjectOfType<AudioConfig>().PlayOneShot("Sword");
                            FindObjectOfType<AudioConfig>().PlayOneShot("MonsterDie");
                            canPlay = false;
                        }
                        
                        StartCoroutine(FadePanel(choosenPath));

                    }
                    else if(choosenPath == 1)
                    {
                        characters[lastPoseIndex].SetActive(false);
                        characters[3].SetActive(true);
                        speakerBox.SetActive(false);
                        speechBox.SetActive(false);
                        FindObjectOfType<AudioConfig> ().StopPlaying ("Background");

                        if (canPlay)
                        {
                            FindObjectOfType<AudioConfig>().PlayOneShot("Friends");
                            canPlay = false;
                        }

                        StartCoroutine(FadePanel(choosenPath));
                    }
                    else if (choosenPath == 2)
                    {
                        characters[lastPoseIndex].SetActive(false);
                        characters[4].SetActive(true);
                        speakerBox.SetActive(false);
                        speechBox.SetActive(false);
                        FindObjectOfType<AudioConfig> ().StopPlaying ("Background");

                        if (canPlay)
                        {
                            FindObjectOfType<AudioConfig>().PlayOneShot("YouDie");
                            canPlay = false;
                        }
                        
                        StartCoroutine(FadePanel(choosenPath));
                    }
                }
            }            
        }
    }
    [HideInInspector] public bool isEnd = false;
    [HideInInspector] public bool isWaitingForUserInput = false;

    [HideInInspector] public bool canPlay = true;
    [HideInInspector] public int lastPoseIndex = 0;
    public int heroIndex = 0;
    public int minotaurIndex = 0;
    public bool isSpeaking()
    {
        if (speaking != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //main dialogue function
    public string targetSpeech = "";
    public string nextSpeaker = "";
    Coroutine speaking = null;
    IEnumerator Speaking (string[][] speakerSpeech, int speakerIndex, string speaker = "")
    {
        speechBox.SetActive(true);
        isWaitingForUserInput = false;
        nextSpeaker = speaker == "Minotauro" ? "Herói" : "Minotauro";

        if (speaker == "")
        {
            speech.fontStyle = FontStyles.Italic;
        }
        else if (speaker == "Herói")
        {
            speakerBox.SetActive(false);
            speech.fontStyle = FontStyles.Bold;
        }
        else if (speaker == "Minotauro")
        {
            speech.fontStyle = FontStyles.Normal;
            speakerBox.SetActive(true);
        }

        for (int i = 0; i < speakerSpeech[speakerIndex].Length; i++)
        {
            SetNextButton(false);
            speech.text = "";
            targetSpeech = speakerSpeech[speakerIndex][i];

            if (speaker == "Minotauro")
            {
                DisplayCharacters(i, speakerIndex);
            }

            if (targetSpeech == pathsSpeech[1][1][1][0])
            {
                characters[lastPoseIndex].SetActive(false);
                characters[0].SetActive(true);
                lastPoseIndex = 0;
            }

            if (targetSpeech == narratorSpeech[0][3])
            {
                FindObjectOfType<AudioConfig>().PlayOneShot("MinotaurSteps");
            }

            if (targetSpeech == narratorSpeech[0][4])
            {
                FindObjectOfType<AudioConfig>().PlayOneShot("MinotaurGrr");
            }

            while(speech.text != targetSpeech)
            {
                speech.text += targetSpeech[speech.text.Length];
                yield return new WaitForSeconds(0.05f);
                yield return new WaitForEndOfFrame();
            }

            isWaitingForUserInput = true;
            SetNextButton(true);

            while (isWaitingForUserInput)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        SetNextButton(false);
        speech.text = "";
        StopSpeaking();
    }

    //stop coroutine
    public void StopSpeaking()
    {
        if(isSpeaking())
        {
            StopCoroutine(speaking);
        }
        speaking = null;
    }

    public void DisplayCharacters(int index, int minotaurIndex)
    {
        characters[lastPoseIndex].SetActive(false);
        characters[minotaurPose[minotaurIndex][index]].SetActive(true);
        lastPoseIndex = minotaurPose[minotaurIndex][index];
    }

    IEnumerator FadePanel(int PanelNumber)
    {
        finalPanels.SetActive(true);
        for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                panels[PanelNumber].color = new Color(1, 1, 1, i);
                yield return null;
            }
    }

    //setting buttons
    void SetNextButton(bool state)
    {
        Button button = nextButton.GetComponent<Button>();
        nextButton.SetActive(state);
        button.interactable = state;
    }

    void SetAnswersButtons(bool state) 
    {
        Button firstButton = firstPath.GetComponent<Button>();
        Button secondButton = secondPath.GetComponent<Button>();
        Button thirdButton = thirdPath.GetComponent<Button>();
        answerChoices.SetActive(state);
        firstButton.interactable = state;
        secondButton.interactable = state;
        thirdButton.interactable = state;

    }

    //clicking buttons
    public void nextButtonSelected()
    {
        FindObjectOfType<AudioConfig>().PlayOneShot("Click");
        isWaitingForUserInput = false;
    }

    int choosenPath = 0;
    public void answerSelected(int index) 
    {
        FindObjectOfType<AudioConfig>().PlayOneShot("Click");
        choosenPath = index;
        minotaurSpeech = pathsSpeech[index][0];
        heroSpeech = pathsSpeech[index][1];
        minotaurPose = pathsPoses [index][0];
        isWaitingForUserInput = false;
        SetAnswersButtons(false);
        nextSpeaker = "Minotauro";
        minotaurIndex = 0;
        heroIndex = 0;
        isEnd = true;
    }

    //intro speech
    string[][] narratorSpeech = {new string[] {"Chega aquela época do ano, temida por todos em Atenas. Quatorze jovens serão levados à Creta como sacrifício ao terrível minotauro.", "Você, um grande herói ateniense, infiltrou-se entre os sacrifícios para enfrentar o monstro meio homem e meio touro e libertar seu povo desse tributo tão cruel.", "Agora, você caminha pelo grande palácio de Knossos, preparado para o inevitável encontro com a besta comedora de gente. Em uma mão, o novelo de Ariadne, que te guiará no caminho de volta, na outra, a espada que dará fim a anos de sofrimento ateniense.", "....", "Ele está vindo! Prepare sua espada!"}};

    string[][] minotaurSpeech = 
    {
        new string[] {"O que pensa que está fazendo com essa arma apontada para mim, tampinha?"},

        new string[] {"Quem?"}, 

        new string[] {"…", "Vá me desculpar, mas eu não como há um ano, e não estou muito no humor para me atualizar sobre o mundo lá fora.", "Você vai ser minha primeira refeição de hoje e eu vou usar essa sua espadinha de palito de dentes e esse novelo de fio dental!", "Aliás, que palhaçada é essa de novelo de lã? Veio aqui tricotar, ó grande herói?"}
    };

    string[][] heroSpeech =
    {
        new string[] {"Me chamo Teseu, filho do rei Egeu de Atenas e do deus dos mares Poseidon, e vim acabar com a carnificina do meu povo!"},

        new string[] {"Não espero que você, enfurnado dentro desse labirinto, saiba quem eu sou, tampouco que tenha ouvido falar de meus grandes feitos.", "Duvido que ao menos tenha ouvido falar dos Argonautas ou da grande guerra contra as Amazonas."}
    };

    //string [path][speaker][speech][]
    string[][][][] pathsSpeech = 
    {
        new string[][][] //path 1
        {
            new string[][] //new minotaurSpeech
            {
                new string[] {"Monstro? Para um nobre você é muito desinformado!", "Os monstros tem uma filiação própria, eles são aqueles nascidos de dois monstros, a Equidna, um monstro metade mulher e metade serpente e de Tifão, um monstro com cem cabeças de serpentes com línguas negras e..."}
            },
            new string[][] //new heroSpeech
            {
                new string[] {"Não me importa! Seja lá o que você for, eu não posso permitir que você continue cometendo atrocidades e trazendo sofrimento para as famílias atenienses!"}
            }
        },
        new string[][][] //path 2
        {
            new string[][] //new minotaurSpeech
            {
                new string[] {"Ariadne é minha irmã!", "Aí você me complica, não posso comer o meu cunhado!", "E agora que você comentou, me deu uma vontade de comer um bem casado…"},
                new string[] {"Claro que não! Eu só como gente porque é a única coisa que me mandam.", "Eu queria mesmo era comer uma pizza, arroz e feijão, guaraná… ", "Suco de caju, goiabada, brigadeiro…"}, 
                new string[] {"Agora somos família! Pode me chamar de Astério, é o meu nome, homenagem ao meu avô."} 
            },
            new string[][] //new heroSpeech
            {
                new string[] {"Bem casado? Achei que você só comia gente!"}, 
                new string[] {"Ô Minotauro, vamos fazer o seguinte então…"}, 
                new string[] {"Então, Astério, vamos sair daqui juntos!", "Seguindo esse fio de novelo a gente encontra o caminho de volta, e você vai poder comer o quanto quiser no buffet do meu casamento!"}
            }
        },
            new string[][][] //path 3
        {
            new string[][] //new minotaurSpeech
            {
                new string[] {"VAQUINHA?"},
                new string[] {"Você é muito do falador, isso sim! Cheio das histórias do mundo lá fora, mas não sabe nada do meu labirinto. Quem ainda não entendeu é você! Você está em desvantagem!"}
            },
            new string[][] //new heroSpeech
            {
                new string[] {"Calma, mimosa! Escuta, eu já ganhei essa parada. Eu sou filho de Poseidon, sou capaz de me comunicar com os deuses, eu tenho ajuda e proteção divina.", "Sou um grande herói guiado por ideais nobres, pela coragem, pela justiça, pelo altruísmo e…"}
            }
        }
    };

    
    //Display Characters Images
    int[][] minotaurPose = 
    {
        new int[] {0},

        new int[] {7}, 

        new int[] {6, 0, 2, 7}
    };

    int[][][][] pathsPoses = 
    {
        new int[][][] //path 1
        {
            new int[][] //Minotaur Poses
            {
                new int[] {0, 0} 
            }
        },
        new int[][][] //path 2
        {
            new int[][] //Minotaur Poses
            {
                new int[] {5, 5, 0},
                new int[] {7, 1, 1},
                new int[] {5}
            }
        },
            new int[][][] //path 3
        {
            new int[][] //Minotaur Poses
            {
                new int[] {2},
                new int[] {2}
            }
        }
    };
        
}
