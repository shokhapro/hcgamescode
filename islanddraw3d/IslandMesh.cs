using UnityEngine;

public class IslandMesh : MonoBehaviour
{
    private const int Size = 128;
    private const float Scale = 0.1f;
    private const float MaxHeight = 0.4f;
    
    [SerializeField] private Texture2D sandBrush;
    
    private MeshFilter _mf;
    private Vector3[] _vertices;
    private Mesh _mesh;

    private void Awake()
    {
        _mf = GetComponent<MeshFilter>();

        InitMesh();
    }
    
    private void InitMesh()
    {
        _vertices = new Vector3[Size * Size];
        for (var x = 0; x < Size; x++)
        for (var y = 0; y < Size; y++)
            _vertices[x * Size + y] = new Vector3(x, 0f, y) * Scale;

        var s = Size - 1;
        int[] triangles = new int[s * s * 6];
        for (var x = 0; x < s; x++)
        for (var y = 0; y < s; y++)
        {
            int i = (x * s + y) * 6;
            triangles[i] = x * s + y;
            triangles[i + 1] = x * s + y + 1;
            triangles[i + 2] = (x + 1) * s + y + 1;
            triangles[i + 3] = x * s + y;
            triangles[i + 4] = (x + 1) * s + y + 1;
            triangles[i + 5] = (x + 1) * s + y;
        }

        _mesh = new Mesh();
        //mesh.indexFormat = _meshIndexFormat;
        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();

        _mf.sharedMesh = _mesh;
    }
    
    public void Sand(Vector3 point, float value)
    {
        var x = Mathf.RoundToInt(point.x / Scale);
        var y = Mathf.RoundToInt(point.z / Scale);
        
        int boffsetx = Mathf.RoundToInt(sandBrush.width / 2f);
        int boffsety = Mathf.RoundToInt(sandBrush.height / 2f);
        for (int bx = 0; bx < sandBrush.width; bx++)
        for (int by = 0; by < sandBrush.height; by++)
        {
            var b = sandBrush.GetPixel(bx, by).grayscale;
            b = Mathf.Clamp01(b - 0.1f);
            
            Add(x + bx - boffsetx, y + by - boffsety, b);
        }
        
        void Add(int xp, int yp, float b)
        {
            if (xp < 0 || xp >= Size || yp < 0 || yp >= Size) return;

            var i = xp * Size + yp;

            var h = _vertices[i].y + value * b;
            h = Mathf.Clamp(h, 0f, MaxHeight);

            _vertices[i].y = h;
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
        
        _mf.sharedMesh = _mesh;
    }

    public void Clear()
    {
        for (var x = 0; x < Size; x++)
        for (var y = 0; y < Size; y++)
            _vertices[x * Size + y].y = 0f;

        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
        
        _mf.sharedMesh = _mesh;
    }

    /*public void SetBrush(Texture2D value)///wtf eto ne realistichno
    {
        activeBrush = value;
    }*/

    public float[] GetHeights()
    {
        var hs = new float[Size * Size];
        
        for (var x = 0; x < Size; x++)
        for (var y = 0; y < Size; y++)
            hs[x * Size + y] = _vertices[x * Size + y].y;
        
        return hs;
    }

    public float GetHeightOnPoint(Vector3 point)
    {
        var x = Mathf.RoundToInt(point.x / Scale);
        var y = Mathf.RoundToInt(point.z / Scale);
        
        if (x < 0 || x >= Size || y < 0 || y >= Size) return 0f;

        return _vertices[x * Size + y].y;
    }
}
