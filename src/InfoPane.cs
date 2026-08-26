namespace EvolutionSim;

public class InfoPane
{
    private bool _infoPaneOnRight = true;
    private int _padding = 4;
    private int _width = 120;
    private int _lineThick = 2;
    private Color _bgCol = new(14, 35, 69, 140);
    private Color _lineCol = new(207, 196, 0);
    private Rectangle _rect;

    //uitext
    private int _textX;
    private int _textY;

    private void LineBreak()
    {
        _textY += FontHeight + 1;
    }
    private void DrawUIText(string text, Color color, int scalar)
    {
        DrawFont(text, color, scalar, _textX, _textY);
        for (int i = 0; i < scalar; i++) LineBreak();
    }
    private void DrawUIText(string text, Color color)
    {
        DrawUIText(text, color, 1);
    }
    private void DrawProgressBar(
        Color color, float part, float whole, string text
    )
    {
        Rectangle barRect = new(
            _rect.X + _padding, 
            _textY, 
            _rect.Width - _padding * 2, 
            14
        );
        float percent = part / whole;
        DrawRectangleRec(barRect, Color.Black);
        DrawRectangleV(
            barRect.Position, 
            barRect.Size * new Vector2(percent, 1), 
            color
        );
        DrawRectangleLinesEx(barRect, 1, Color.Black);
        DrawFontV(text, Color.White, 1, barRect.Position);
        LineBreak();
    }

    private void DrawFood(Food food)
    {
        DrawUIText($"type: {food.Type}", Color.White);
        DrawUIText($"stage: {food.Stage}", Color.White);
    }

    private void DrawAnimal(Animal animal)
    {
        // color speed sight hunger
        DrawUIText(animal.Name, Color.White);
        LineBreak();

        DrawRectangle(_textX + 40, _textY, 12, 12, animal.Color);
        DrawUIText("color:", animal.Color);
        
        DrawUIText($"sight: {animal.Sight}", Color.White);
        DrawUIText($"speed: {animal.Speed}", Color.White);
        LineBreak();

        if (animal.ReadyToMate)
        {
            DrawUIText("Ready To Mate", Color.Pink);
        }
        else
        {
            LineBreak();
        }

        //these should be bars
        //hunger bar
        foreach (BarItem bar in animal.BarValues)
            DrawProgressBar(bar.Color, bar.Part, bar.Whole, bar.Priority.ToString());
        DrawProgressBar(Color.Green, animal.Health, Animal.MaxHealth, "health");
        //thirst bar
        // then, create reproduction bar, maybe intelligence bar?
    }

    public void Draw(Entity? hover, Camera2D cam)
    {
        if (hover == null) return;

        _rect = new(
            _padding, _padding, 
            _width, WindowHeight - (_padding * 2)
        );
        Vector2 lineStart = new(
            _rect.X + _rect.Width,
            _rect.Y + 50
        );
        Vector2 lineEnd = (hover.Position - cam.Target) * cam.Zoom / WindowScale;
        if (_infoPaneOnRight)
        {
            _rect.X = WindowWidth - _rect.Width - _padding;
            lineStart.X = _rect.X;
        }

        if (CheckCollisionPointRec(GetMousePosition() / WindowScale, _rect))
            _infoPaneOnRight = !_infoPaneOnRight;

        DrawLineEx(lineStart, lineEnd, _lineThick, _lineCol);
        DrawRectangleRec(_rect, _bgCol);
        DrawRectangleLinesEx(_rect, _lineThick, _lineCol);

        _textX = (int)_rect.Position.X + _padding;
        _textY = 8;
        DrawUIText(hover.GetType().Name, Color.White, 2);
        DrawUIText(
            $"{hover.Age.Minutes:D2}:{hover.Age.Seconds:D2} seconds old", 
            Color.Gray
        );
        LineBreak();

        if (hover is Food food) DrawFood(food);
        if (hover is Animal animal) DrawAnimal(animal);
    }
}