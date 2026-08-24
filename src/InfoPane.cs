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

    private void DrawUIText(string text, Color color, int scalar)
    {
        DrawFont(text, color, scalar, _textX, _textY);
        _textY += (FontHeight * scalar) + _padding;
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
        _textY += FontHeight + _padding;
    }

    private void DrawFood(Food food)
    {
        DrawUIText($"type: {food.Type}", Color.Gray);
        DrawUIText($"stage: {food.Stage}", Color.Gray);
    }

    private void DrawAnimal(Animal animal)
    {
        // color speed sight hunger
        DrawUIText(animal.Name, Color.White);

        DrawRectangle(_textX + 40, _textY, 12, 12, animal.Color);
        DrawUIText("color:", animal.Color);
        
        DrawUIText($"sight: {animal.Sight}", Color.Gray);
        DrawUIText($"speed: {animal.Speed}", Color.Gray);

        //these should be bars
        //hunger bar
        DrawProgressBar(
            Color.DarkGreen, 
            animal.Health, 
            animal.MaxHealth,
            "health"
        );
        DrawProgressBar(
            Color.DarkBrown, 
            animal.Hunger, 
            animal.MaxHunger,
            "hubger"
        );
        DrawProgressBar(
            Color.DarkBlue, 
            animal.Thirst, 
            animal.MaxThirst,
            "thirst"
        );
        //thirst bar
        // then, create reproduction bar, maybe intelligence bar?
    }

    public void Draw(Entity? hover)
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
        Vector2 lineEnd = new(
            hover.Position.X - hover.FrameSize.X / 2,
            hover.Position.Y - hover.FrameSize.Y / 2
        );
        if (_infoPaneOnRight)
        {
            _rect.X = WindowWidth - _rect.Width - _padding;
            lineStart.X = _rect.X;
            lineEnd.X = hover.Position.X + hover.FrameSize.X / 2;
        }

        if (CheckCollisionPointRec(GetMousePosition() / WindowScale, _rect))
            _infoPaneOnRight = !_infoPaneOnRight;

        DrawRectangleRec(_rect, _bgCol);
        DrawRectangleLinesEx(_rect, _lineThick, _lineCol);
        DrawLineEx(lineStart, lineEnd, _lineThick, _lineCol);

        _textX = (int)_rect.Position.X + _padding;
        _textY = 8;
        DrawUIText(hover.GetType().Name, Color.White, 2);
        DrawUIText(
            $"{hover.Age.Minutes:D2}:{hover.Age.Seconds:D2} seconds old", 
            Color.Gray
        );

        if (hover is Food food) DrawFood(food);
        if (hover is Animal animal) DrawAnimal(animal);
    }
}