using System.Text;

public class TextBox
{
    private StringBuilder _text;
    private int _maxChars = 4;
    private bool _isFocused;
    private string _label;

    public Rectangle Bounds { get; set; }
    public string Text => _text.ToString();
    public bool IsFocused => _isFocused;

    public int IntValue
    {
        get
        {
            if (int.TryParse(_text.ToString(), out int result))
            {
                return result;
            }
            return 0;
        }
    }

    public TextBox(string label, float x, float y, float width, float height)
    {
        Bounds = new Rectangle(x, y, width, height);
        _label = label;
        _text = new StringBuilder(_maxChars);
        _isFocused = false;
    }

    public void Update()
    {
        if (IsMouseButtonPressed(MouseButton.Left))
            _isFocused = CheckCollisionPointRec(GetMousePosition() / WindowScale, Bounds);
        

        if (_isFocused)
        {
            int key = GetCharPressed();
            while (key > 0)
            {
                if (_text.Length < _maxChars)
                {
                    if (key >= '0' && key <= '9')
                        _text.Append((char)key);
                }
                key = GetCharPressed();
            }

            if (IsKeyPressed(KeyboardKey.Backspace) && _text.Length > 0)
            {
                _text.Length--;
            }
        }
    }

    public void Draw()
    {
        Color borderColor = _isFocused ? Color.Red : Color.DarkGray;

        DrawRectangleRec(Bounds, Color.Gray);
        DrawRectangleLines(
            (int)Bounds.X, (int)Bounds.Y, 
            (int)Bounds.Width, (int)Bounds.Height, 
            borderColor
        );
        DrawFont(
            _text.ToString(), Color.Black, 1, 
            (int)Bounds.X + 8, (int)Bounds.Y + ((int)Bounds.Height / 2) - 10
        );
        DrawFont(
            _label, Color.White, 1, 
            (int)Bounds.X + 8, (int)Bounds.Y - ((int)Bounds.Height / 2) - 10
        );
    }

    public void Clear()
    {
        _text.Clear();
    }
}
