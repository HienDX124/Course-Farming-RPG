using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField] private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        // Only populate in editor
        if (!Application.IsPlaying(gameObject))
        {
            // Ensure the object has guaranted unique id
            if (_gUID == "")
            {
                // Assgign GUID
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
