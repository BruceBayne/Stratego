using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Stratego.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Figure : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshPro figureName;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material GlassMaterial;


    private StrategoTypes.FieldSlot slot;
    private StrategoTypes.Figure type;
    private StrategoTypes.FigurePosition CurrentPosition => StrategoTypes.FigurePosition.Create(xPos, yPos).Value;


    private int xPos;
    private int yPos;


    public StrategoTypes.FigurePosition Position => StrategoTypes.FigurePosition.Create(xPos, yPos).Value;

    // Start is called before the first frame update
    void Start()
    {
    }


    public void Setup(StrategoTypes.FieldSlot slot, int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;

        transform.name = $"X:{xPos} Y:{yPos} {slot}";


        figureName.text = string.Empty;

        if (slot.TryGetFigure(out type))
            figureName.text = type.Rank.ToString();


        if (slot.IsEmpty)
        {
            figureName.text = "E";
            meshRenderer.enabled = false;
            //gameObject.SetActive(false);
        }

        if (slot.IsObstacle)
        {
            meshRenderer.material = GlassMaterial;
            figureName.text = "[O]";
        }

        this.slot = slot;
    }

    // Update is called once per frame
    void Update()
    {
    }


    public bool IsEnemyFigure => slot.IsFigure && type.Owner.Equals(StrategoTypes.Player.Red);
    public bool IsOwnFigure => slot.IsFigure && type.Owner.Equals(StrategoTypes.Player.Blue);


    public bool ShouldMove()
    {
        if (BoardLogic.CurrentSelectedFigure == null)
            return false;

        return BoardLogic.CurrentSelectedFigure.CanMoveTo(xPos, yPos);
    }

    private bool CanMoveTo(int destinationX, int destinationY)
    {
        var currentPosition = StrategoTypes.FigurePosition.Create(xPos, yPos);
        var possibleMoves =
            StrategoLogic.CalculateAvailableMoves(BoardLogic.CurrentGame.GameField.Field, currentPosition.Value);

        if (possibleMoves.IsOk)
        {
            return possibleMoves.ResultValue.Any(x =>
                x.Equals(StrategoTypes.FigurePosition.Create(destinationX, destinationY).Value));
        }

        return false;
    }

    public bool ShouldHighlightPossibleMoves()
    {
        return IsOwnFigure;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (ShouldHighlightPossibleMoves())
        {
            var currentPosition = StrategoTypes.FigurePosition.Create(xPos, yPos);
            var possibleMoves =
                StrategoLogic.CalculateAvailableMoves(BoardLogic.CurrentGame.GameField.Field, currentPosition.Value);
            BoardLogic.CurrentSelectedFigure = this;

            if (possibleMoves.IsOk)
                HighlightPossibleMoves(possibleMoves.ResultValue);
            return;
        }






        if (ShouldMove())
        {
            BoardLogic.CurrentSelectedFigure.MoveTo(CurrentPosition);
        }
    }

    private void MoveTo(StrategoTypes.FigurePosition newPosition)
    {

        var moveResult = StrategoLogic.MakeMove(BoardLogic.CurrentGame, new StrategoTypes.MoveIntent(StrategoTypes.Player.Blue, CurrentPosition, newPosition));

        if (!moveResult.IsOk)
        {
            BoardLogic.Log(moveResult.ErrorValue);
            return;
        }

        BoardLogic.Log(moveResult.ResultValue);

        if (moveResult.ResultValue.TurnInfo.IsJustMoveCase)
        {
            BoardLogic.Log("Just move case:" + moveResult.ResultValue.TurnInfo);


            BoardLogic.CurrentSelectedFigure = null;



        }

        if (moveResult.ResultValue.TurnInfo.IsDeathCase)
        {
                
        }
    }

    private void HighlightPossibleMoves(IEnumerable<StrategoTypes.FigurePosition> possibleMoves)
    {
        foreach (var possibleMove in possibleMoves)
        {
            Debug.Log(possibleMove.ToString());
        }
    }
}