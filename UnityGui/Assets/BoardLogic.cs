using Assets;
using Assets.Prefabs;
using Stratego.Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardLogic : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private GameObject figuresRoot;
    [SerializeField] private Text currentPlayer;

    
    public static StrategoTypes.GameInformation CurrentGame;


    public static GameObject FigurePrefab { get; private set; }
    

    public static void Log(object o)
    {
        Log(o.ToString());
    }

    public static void Log(string message)
    {
        Debug.Log(message);

    }

   public void RepaintFigures()
    {

        figuresRoot.transform.DestroyChild();
        currentPlayer.text = CurrentGame.CurrentPlayer.ToString();

        for (var x = 0; x < CurrentGame.GameField.Field.GetLength(0); x++)
        {
            for (var y = 0; y < CurrentGame.GameField.Field.GetLength(1); y++)
            {

                var slot = CurrentGame.GameField.Field[x, y];

                if(slot.IsEmpty)
                    continue;
                
                var figure = Instantiate(figurePrefab, figuresRoot.transform, false);
                figure.GetComponent<Figure>().Setup(GetFieldSlotWithPos(x,y));

            }
        }
        
    }


   public static FieldSlotWithPos GetFieldSlotWithPos(StrategoTypes.FigurePosition position)
   {
       var (x, y) = position.ToXandY();
       return new FieldSlotWithPos(CurrentGame.GameField.Field[x, y], x, y);

   }


    public static FieldSlotWithPos GetFieldSlotWithPos(int x, int y)
   {
       return new FieldSlotWithPos(CurrentGame.GameField.Field[x, y],x,y);

   }
  

    // Start is called before the first frame update
    void Start()
    {
        FigurePrefab = figurePrefab;
        var board=FieldGenerator.CreateRandomBoard(40);
        SetupCurrentGame(FieldGenerator.StartGame(board));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        



    }

    private void SetupCurrentGame(StrategoTypes.GameInformation newInfo)
    {
        CurrentGame = newInfo;
        RepaintFigures();
        
    }
}
