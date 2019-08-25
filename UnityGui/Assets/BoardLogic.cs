using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Microsoft.FSharp.Core;
using Stratego.Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardLogic : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private GameObject figuresRoot;
    [SerializeField] private Text currentPlayer;



    [SerializeField] private GameObject bottomLeftCorner;


    public static StrategoTypes.GameInformation CurrentGame;


    public static Figure CurrentSelectedFigure;



    public static void Log(object o)
    {
        Log(o.ToString());
    }

    public static void Log(string message)
    {
        Debug.Log(message);

    }

    void RepaintFigures()
    {

        figuresRoot.transform.DestroyChild();
        currentPlayer.text = CurrentGame.CurrentPlayer.ToString();

        for (int x = 0; x < CurrentGame.GameField.Field.GetLength(0); x++)
        {
            for (int y = 0; y < CurrentGame.GameField.Field.GetLength(1); y++)
            {
                var slot = CurrentGame.GameField.Field[x, y];
                SpawnFigureAt(slot,x,y);
            }
        }
        
    }

    private void SpawnFigureAt(StrategoTypes.FieldSlot slot, int xPos, int yPos)
    {
        var figure = Instantiate(figurePrefab, figuresRoot.transform, false);


        var xPosDelta = xPos+(xPos*0.11f);
        var zPosDelta = yPos+(yPos*0.2f);

        var figurePosition = new Vector3(bottomLeftCorner.transform.position.x+xPosDelta, bottomLeftCorner.transform.position.y, bottomLeftCorner.transform.position.z+zPosDelta);

        figure.transform.position = figurePosition;
        figure.GetComponent<Figure>().Setup(slot,xPos,yPos);

    }


    // Start is called before the first frame update
    void Start()
    {
        
        SetupCurrentGame(StrategoLogic.StartPredefinedGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if(CurrentSelectedFigure==null)
        //    return;


        //var from = CurrentSelectedFigure.Position;
        //var intent = new StrategoTypes.MoveIntent(StrategoTypes.Player.Blue, from, from);
        //var moveResult=StrategoLogic.MakeMove(CurrentGame, intent);


        
        //if (moveResult.IsOk)
        //{
        //    Log(moveResult.ResultValue.ToString());
        //    SetupCurrentGame(moveResult.ResultValue.GameInfo);
        //}
        //else
        //{
        //    Log(moveResult.ErrorValue.ToString());
        //}



    }

    private void SetupCurrentGame(StrategoTypes.GameInformation newInfo)
    {
        CurrentSelectedFigure = null;
        CurrentGame = newInfo;
        RepaintFigures();
        
    }
}
