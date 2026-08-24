namespace EvolutionSim;

public class InfoPane
{
    private bool _infoPaneOnRight = true;

    private void DrawProgressBar(
        Rectangle rect, 
        Color color,
        float part, 
        float whole,
        string text
    )
    {
        float percent = part / whole;
        DrawRectangleRec(rect, Color.Black);
        DrawRectangleV(
            rect.Position, 
            rect.Size * new Vector2(percent, 1), 
            color
        );
        DrawRectangleLinesEx(rect, 1, Color.Black);
        DrawFontV(text, Color.White, 1, rect.Position);
    }

    public void Draw(Entity? hover)
    {
        if (hover == null) return;
    
        int infoPanePadding = 4;
        int infoPaneWidth = 120;
        int infoPaneLineThickness = 2;
        Color infoPaneBgColor = new(14, 35, 69, 140);
        Color infoPaneLineColor = new(207, 196, 0);

        Rectangle infoPaneRect = new(
            infoPanePadding, 
            infoPanePadding, 
            infoPaneWidth, 
            WindowHeight - (infoPanePadding * 2)
        );

        int mouseX = GetMouseX() / WindowScale;
        if (mouseX < infoPaneRect.X + infoPaneRect.Width)
            _infoPaneOnRight = true;
        if (mouseX > WindowWidth - infoPaneRect.Width - infoPanePadding)
            _infoPaneOnRight = false;

        Vector2 lineStart = new(
            infoPaneRect.X + infoPaneRect.Width,
            infoPaneRect.Y + 50
        );
        Vector2 lineEnd = new(
            hover.Position.X - hover.FrameSize.X / 2,
            hover.Position.Y - hover.FrameSize.Y / 2
        );
        
        if (_infoPaneOnRight)
        {
            infoPaneRect.X = 
                WindowWidth - 
                infoPaneRect.Width - 
                infoPanePadding;
            lineStart.X = infoPaneRect.X;
            lineEnd.X = hover.Position.X + hover.FrameSize.X / 2;
        }

        DrawRectangleRec(infoPaneRect, infoPaneBgColor);
        DrawRectangleLinesEx(
            infoPaneRect, 
            infoPaneLineThickness, 
            infoPaneLineColor
        );
        DrawLineEx(
            lineStart, 
            lineEnd, 
            infoPaneLineThickness, 
            infoPaneLineColor
        );

        int infoPaneTextX = (int)infoPaneRect.Position.X + infoPanePadding;
        DrawFont(hover.GetType().Name, 
            Color.White, 2, infoPaneTextX, 6
        );
        DrawFont(
            $"{hover.Age.Minutes:D2}:{hover.Age.Seconds:D2} seconds old",
            Color.Gray, 1, infoPaneTextX, 32
        );
        if (hover is Food food)
        {
            DrawFont($"type: {food.Type}", 
                Color.Gray, 1, infoPaneTextX, 42
            );
            DrawFont($"stage: {food.Stage}", 
                Color.Gray, 1, infoPaneTextX, 52
            );
        }
        if (hover is Animal animal)
        {
            // color speed sight hunger
            DrawFont("color:", 
                animal.Color, 1, infoPaneTextX, 42
            );
            DrawRectangle(infoPaneTextX + 40, 44, 10, 10, animal.Color);
            DrawFont($"sight: {animal.Sight}", 
                Color.Gray, 1, infoPaneTextX, 52
            );
            DrawFont($"speed: {animal.Speed}", 
                Color.Gray, 1, infoPaneTextX, 62
            );
            //these should be bars
            
            //hunger bar
            Rectangle barRect = new(
                infoPaneRect.X + infoPanePadding, 
                82, 
                infoPaneRect.Width - infoPanePadding * 2, 
                14
            );
            DrawProgressBar(
                barRect, 
                Color.DarkGreen, 
                animal.Health, 
                animal.MaxHealth,
                "health"
            );
            barRect.Y = 98;
            DrawProgressBar(
                barRect, 
                Color.DarkBrown, 
                animal.Hunger, 
                animal.MaxHunger,
                "hubger"
            );
            barRect.Y = 114;
            DrawProgressBar(
                barRect, 
                Color.DarkBlue, 
                animal.Thirst, 
                animal.MaxThirst,
                "thirst"
            );
            //thirst bar
            // then, create reproduction bar, maybe intelligence bar?
        }
    }
}