using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [Header("Board")]
    public Board Board;
    public CandySwipeSettings CandySwipeSettings;
    public List<GameObject> candiesToSpawn;
    public float spawnOffset;
    [Header("Match")]
    public MatchCounter MatchCounter;
    [Header("Lose Condition")]
    public LoseConditionType Type;
    [Header("Tiles")]
    public GameObject basicTile;
    public GameObject chocolateTile;

    public override void InstallBindings()
    {
        LoseConditionInstaller.Install(Container, Type);
        BoardInstaller.Install(Container, Board, basicTile, chocolateTile, candiesToSpawn, spawnOffset);
        CandyInstaller.Install(Container, CandySwipeSettings);
        MatchSystemInstaller.Install(Container, MatchCounter);
    }
}
public class LoseConditionInstaller : Installer<LoseConditionType, LoseConditionInstaller>
{
    private LoseConditionType type;
    public LoseConditionInstaller(LoseConditionType type)
    {
        this.type = type;
    }

    public override void InstallBindings()
    {
        Container.Bind<LoseConditionType>().FromInstance(type).AsSingle();
        Container.BindFactory<int, TMP_Text, TMP_Text, LoseConditionBase, LoseConditionBase.Factory>().FromFactory<CustomLoseConditionFactory>();
    }
}
public class BoardInstaller : Installer<Board, GameObject, GameObject, List<GameObject>, float, BoardInstaller>
{
    private Board board;
    private GameObject basicTile;
    private GameObject chocolateTile;
    private List<GameObject> candiesToSpawn;
    private float spawnOffset;
    public BoardInstaller(Board board, GameObject basicTile, GameObject chocolateTile, List<GameObject> candiesToSpawn, float spawnOffset)
    {
        this.board = board;
        this.basicTile = basicTile;
        this.chocolateTile = chocolateTile;
        this.candiesToSpawn = candiesToSpawn;
        this.spawnOffset = spawnOffset;
    }
    public override void InstallBindings()
    {
        Container.Bind<Board>().FromInstance(board).AsSingle();
        Container.Bind<ITileController>().To<DefaultTileController>().AsSingle().WithArguments(basicTile, chocolateTile);
        Container.Bind<ISwipeController>().To<DeaulfSwipeController>().AsSingle();
        Container.Bind<IBoardLoopController>().To<DefaultBoardLoopController>().AsSingle().WithArguments(candiesToSpawn, spawnOffset);
    }
}
public class CandyInstaller : Installer<CandySwipeSettings, CandyInstaller>
{
    private CandySwipeSettings candySwipeSettings;
    public CandyInstaller(CandySwipeSettings candySwipeSettings)
    {
        this.candySwipeSettings = candySwipeSettings;
    }
    public override void InstallBindings()
    {
        Container.Bind<IMatchMaker>().To<DefaultMatchMaker>().AsSingle();
        Container.Bind<IMatchFinder>().To<BasicCandyMatchFinder>().AsSingle().WhenNotInjectedInto<ColorBomb>();
        Container.Bind<IMatchFinder>().To<DummyMatchFinder>().AsSingle().WhenInjectedInto<ColorBomb>();
        Container.Bind<ISwipeHandler>().To<SwipeHandlerTangent>().AsSingle();
        Container.Bind<CandySwipeSettings>().FromInstance(candySwipeSettings).AsSingle();
    }
}
public class MatchSystemInstaller : Installer<MatchCounter, MatchSystemInstaller>
{
    private MatchCounter matchCounter;
    public MatchSystemInstaller(MatchCounter matchCounter)
    {
        this.matchCounter = matchCounter;
    }
    public override void InstallBindings()
    {
        Container.Bind<MatchCounter>().FromInstance(matchCounter).AsSingle();
    }
}