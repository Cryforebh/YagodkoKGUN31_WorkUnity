using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float _valumeUnits = 1f;
    private float _valumeOther = 1f;
    private float _valumeAmbient = 1f;
    private float _valumeModifications = 1f;

    public float ValumeUnits { get => _valumeUnits; set => _valumeUnits = value; }
    public float ValumeOther { get => _valumeOther; set => _valumeOther = value; }
    public float ValumeAmbient { get => _valumeAmbient; set => _valumeAmbient = value; }
    public float ValumeModifications { get => _valumeModifications; set => _valumeModifications = value; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}

public enum EnumSounds
{
    None = 0,
    Units = 1,
    Modifications = 2,
    Ambient = 3,
    Other = 4,
}
