using System.Collections;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance;
    [SerializeField] private float timeBeetwenHintControls;
    [SerializeField] private GameObject rightHintParticle;
    [SerializeField] private GameObject upHintParticle;
    private GameObject hintParticle;
    private bool isParticleActive;
    private float elapsedTime;
    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
    }

    private void Start()
    {
        Board.Instance.OnSwap += ResetTimer;
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
        for (int i = 0; i < Board.Instance.Width; i++)
        {
            for (int j = 0; j < Board.Instance.Height; j++)
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

        if (column < Board.Instance.Width - 1 && Board.Instance.IsLegalMovementTile(column + 1, row)
            && Board.Instance.IsLegalMovementTile(column, row))
        {
            Candy source = Board.Instance.Columns[column].rows[row].GetComponent<Candy>();
            Candy right = Board.Instance.Columns[column + 1].rows[row].GetComponent<Candy>();

            Board.Instance.SwitchPieces(column, row, Vector2.right);

            bool sourceMatch = source.FindMatch(false);
            bool rightMatch = right.FindMatch(false);

            if (sourceMatch | rightMatch)
            {
                result = true;
                hintParticle = Instantiate(rightHintParticle, new Vector2(column, row), Quaternion.identity);
                isParticleActive = true;
            }

            Board.Instance.SwitchPieces(column, row, Vector2.right);
        }

        return result;
    }
    private bool ControlUpHint(int column, int row)
    {
        bool result = false;

        if (row < Board.Instance.Height - 1 && Board.Instance.IsLegalMovementTile(column, row)
            && Board.Instance.IsLegalMovementTile(column, row + 1))
        {
            Candy source = Board.Instance.Columns[column].rows[row].GetComponent<Candy>();
            Candy up = Board.Instance.Columns[column].rows[row + 1].GetComponent<Candy>();
            Board.Instance.SwitchPieces(column, row, Vector2.up);
            bool sourceMatch = source.FindMatch(false);
            bool rightMatch = up.FindMatch(false);

            if (sourceMatch | rightMatch)
            {
                result = true;
                hintParticle = Instantiate(upHintParticle, new Vector2(column, row), upHintParticle.transform.rotation);
                isParticleActive = true;
            }

            Board.Instance.SwitchPieces(column, row, Vector2.up);
        }

        return result;
    }
    private void DeleteParticle()
    {
        Destroy(hintParticle);
        isParticleActive = false;
    }
}
