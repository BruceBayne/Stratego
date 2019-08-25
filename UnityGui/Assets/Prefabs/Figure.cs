using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Prefabs;
using Microsoft.FSharp.Core;
using Stratego.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class Figure : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshPro figureName;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material GlassMaterial;


    [SerializeField] private Material SelectedMat;

    private Material startMaterial;
    public static Figure CurrentSelectedFigure;


    private GameObject rootObject => transform.parent.gameObject;
    private Vector3 rootPosition => rootObject.transform.position;
    private FieldSlotWithPos slot;
    private StrategoTypes.Figure type;




    public bool IsEmptySlot => slot.Slot.IsEmpty;

    // Start is called before the first frame update
    void Start()
    {
        startMaterial = meshRenderer.material;
    }


    public void UpdateFigure(FieldSlotWithPos slot)
    {
        this.slot = slot;

        var xPosDelta = slot.X + (slot.X * 0.14f);
        var zPosDelta = slot.Y + (slot.Y * 0.14f);
        var figurePosition = new Vector3(rootPosition.x + xPosDelta, rootPosition.y, rootPosition.z + zPosDelta);
        transform.position = figurePosition;

        

        if (this.slot.Slot.TryGetFigure(out type))
        {
            figureName.text = type.Rank.ToString();
            transform.name = $"{type.Owner}/{type.Rank} / {slot}";


            
            if (type.Owner.Equals(StrategoTypes.Player.Blue))
                transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (this.slot.Slot.IsEmpty)
        {
            name = $"Empty : {slot}";
            figureName.text = "E";
            meshRenderer.enabled = false;
        }

        if (this.slot.Slot.IsObstacle)
        {
            name = $"Obstacle :{slot}";
            gameObject.SetActive(false);
            meshRenderer.material = GlassMaterial;
            figureName.text = "[O]";
        }
    }


    public void Setup(FieldSlotWithPos slot)
    {
      
        figureName.text = string.Empty;
        UpdateFigure(slot);
    }


    public bool PlayerWantToMove()
    {
        if (CurrentSelectedFigure == null)
            return false;

        return CurrentSelectedFigure.CanMoveTo(slot.Position);
    }

    private bool CanMoveTo(StrategoTypes.FigurePosition position)
    {
        var possibleMoves =
            StrategoLogic.CalculateAvailableMoves(BoardLogic.CurrentGame.GameField.Field, slot.Position);

        if (possibleMoves.IsOk)
        {
            return possibleMoves.ResultValue.Any(x => x.Equals(position));
        }

        return false;
    }

    public bool CanPickFigure()
    {
        return true;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        ResetCurrentHighlight();

        if (PlayerWantToMove())
        {
            CurrentSelectedFigure.MoveTo(slot.Position);
            return;
        }

        if (CanPickFigure())
        {
            PickCurrentFigure();
        }
    }



    private Figure LocateFigure(Predicate<Figure> predicate)
    {
        return rootObject.GetComponentsInChildren<Figure>().FirstOrDefault(f=>predicate(f));

    }


    private Figure LocateFigure(StrategoTypes.FigurePosition position)
    {
        return LocateFigure(x => x.slot.Position.Equals(position));

    }



    private void ClearEmptySlots()
    {
        var allFigures = rootObject.GetComponentsInChildren<Figure>().Where(x => x.IsEmptySlot);

        foreach (var figure in allFigures.Select(x => x.gameObject))
            Destroy(figure);
    }


    private void SetNothingSelected()
    {
        ResetCurrentHighlight();
        CurrentSelectedFigure = null;
    }


    private void ResetCurrentHighlight()
    {
        if (CurrentSelectedFigure != null)
            CurrentSelectedFigure.meshRenderer.material = startMaterial;
        ClearEmptySlots();
    }

    private void PickCurrentFigure()
    {
        CurrentSelectedFigure = this;
        meshRenderer.material = SelectedMat;
        var possibleMoves = GetPossibleMoves();
        if (possibleMoves.IsOk)
            HighlightPossibleMoves(possibleMoves.ResultValue);
    }

    private FSharpResult<IEnumerable<StrategoTypes.FigurePosition>, StrategoTypes.TurnErrorInfo> GetPossibleMoves()
    {
        var possibleMoves =
            StrategoLogic.CalculateAvailableMoves(BoardLogic.CurrentGame.GameField.Field, slot.Position);
        return possibleMoves;
    }

    private void MoveTo(StrategoTypes.FigurePosition newPosition)
    {
        var moveResult = StrategoLogic.MakeMove(BoardLogic.CurrentGame,
            new StrategoTypes.MoveIntent(this.type.Owner, slot.Position, newPosition));

        if (!moveResult.IsOk)
        {
            BoardLogic.Log(moveResult.ErrorValue);
            return;
        }

        var resultValue = moveResult.ResultValue;
        var turnInfo = resultValue.TurnInfo;


        BoardLogic.Log(resultValue);
        BoardLogic.CurrentGame = resultValue.GameInfo;

        
        if (turnInfo.TryGetDeathCase(out var t))
        {
            var (moveInfo, killInfo, rank) = (t.Item1,t.Item2,t.Item3);

            BoardLogic.Log("Death Case:" + turnInfo);

            var killSelf = killInfo.IsMyFigureDead || killInfo.IsBothDead;
            var killEnemy = killInfo.IsBothDead || killInfo.IsIAmKiller;


            if (killSelf)
                Die();

            if (killEnemy)
            {
                var figureToKill=LocateFigure(moveInfo.NewPosition);
                figureToKill.Die();
            }
            if(!killSelf)
                Move(moveInfo);
            
        }

        
        if (turnInfo.IsJustMoveCase)
        {
            turnInfo.TryGetJustMoveCase(out var moveInfo);
            Move(moveInfo);
            BoardLogic.Log("Just move case:" + turnInfo);
        }

        
    }

    private void Move(StrategoTypes.MoveInfo moveInfo)
    {
        if (moveInfo != null)
            UpdateFigure(BoardLogic.GetFieldSlotWithPos(moveInfo.NewPosition));

        MoveSuccess();
    }



    IEnumerator ScaleToZero()
    {

        var x = 25;

        while (x > 0)
        {
            yield return new WaitForSeconds(0.05f);
            gameObject.transform.localScale=new Vector3(gameObject.transform.localScale.x*0.75f, gameObject.transform.localScale.y * 0.75f, gameObject.transform.localScale.z * 0.75f);
            x--;
        }
        
        Destroy(gameObject);

    }

    private void Die()
    {
        MoveSuccess();


        StartCoroutine(ScaleToZero());
        

    }

    private void MoveSuccess()
    {
        SetNothingSelected();
    }


    private void HighlightPossibleMoves(IEnumerable<StrategoTypes.FigurePosition> possibleMoves)
    {
        foreach (var possibleMove in possibleMoves)
        {
            //we need to spawn Empty Slots here (to add collider onClick) + highLight somehow


            var (x, y) = possibleMove.ToXandY();


            //We are not require Empty Slot for Attack Figure
            if (BoardLogic.CurrentGame.GameField.Field[x, y].TryGetFigure(out var f))
            {
                continue;
            }

            

            var figure = Instantiate(BoardLogic.FigurePrefab, rootObject.transform, false);
            figure.GetComponent<Figure>().Setup(BoardLogic.GetFieldSlotWithPos(possibleMove));


            Debug.Log(possibleMove.ToString());
        }
    }
}