using System.Collections;
using UnityEngine;
using Zenject;

public class HintManager : MonoBehaviour
{
    [SerializeField] private float timeBeetwenHintControls;
    [SerializeField] private GameObject rightHintParticle;
    [SerializeField] private GameObject upHintParticle;
    private GameObject hintParticle;
    private bool isParticleActive;
    private float elapsedTime;
    private Board board;

    [Inject]
    public void Construct(Board board)
    {
        this.board = board;
    }

    private void Start()
    {
        board.OnSwap += ResetTimer;
        StartCoroutine(ControlHintCo());
    }
    private void ResetTimer()
    {
        if (isParticleActive) DeleteParticle();
        elapsedTime = 0;
    }
    private IEnumerator ControlHintCo()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (!isParticleActive && elapsedTime >= timeBeetwenHintControls)
            {
                ControlHint();
                elapsedTime = 0;
            }

            yield return null;
        }
    }
    private void ControlHint()
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                if (ControlRightHint(i, j) || ControlUpHint(i, j))
                {
                    return;
                }
            }
        }
    }
    private bool ControlRightHint(int column, int row)
    {
        bool result = false;

        if (column < board.Width - 1 && board.IsLegalMovementTile(column + 1, row)
            && board.IsLegalMovementTile(column, row))
        {
            Candy source = board.Columns[column].rows[row].GetComponent<Candy>();
            Candy right = board.Columns[column + 1].rows[row].GetComponent<Candy>();

            board.SwitchPieces(column, row, Vector2.right);

            bool sourceMatch = source.FindMatch(false);
            bool rightMatch = right.FindMatch(false);

            if (sourceMatch | rightMatch)
            {
                result = true;
                hintParticle = Instantiate(rightHintParticle, new Vector2(column, row), Quaternion.identity);
                isParticleActive = true;
            }

            board.SwitchPieces(column, row, Vector2.right);
        }

        return result;
    }
    private bool ControlUpHint(int column, int row)
    {
        bool result = false;

        if (row < board.Height - 1 && board.IsLegalMovementTile(column, row)
            && board.IsLegalMovementTile(column, row + 1))
        {
            Candy source = board.Columns[column].rows[row].GetComponent<Candy>();
            Candy up = board.Columns[column].rows[row + 1].GetComponent<Candy>();
            board.SwitchPieces(column, row, Vector2.up);
            bool sourceMatch = source.FindMatch(false);
            bool rightMatch = up.FindMatch(false);

            if (sourceMatch | rightMatch)
            {
                result = true;
                hintParticle = Instantiate(upHintParticle, new Vector2(column, row), upHintParticle.transform.rotation);
                isParticleActive = true;
            }

            board.SwitchPieces(column, row, Vector2.up);
        }

        return result;
    }
    private void DeleteParticle()
    {
        Destroy(hintParticle);
        isParticleActive = false;
    }
}
