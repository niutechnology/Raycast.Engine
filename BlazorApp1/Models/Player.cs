using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApp1.Models
{
    public class Character
    {
        public Position Position { get; set; }
        public float Size { get; set; }
        public string Color { get; set; }
        public float MovementSpeed { get; set; }
        public Position PreviousPosition { get; private set; }
        public Position TargetPosition { get; set; }
        public Character()
        {
            Position = new Position(0, 0);
            PreviousPosition = new Position(0, 0);
        }

        public void UpdatePreviousPosition()
        {
            PreviousPosition = new Position(Position.X, Position.Y);
        }

        public void Move(float canvasWidth)
        {
            this.Position.X += MovementSpeed;

            if (this.Position.X > canvasWidth - Size)
            {
                this.Position.X = 0;
            }
        }

        public async Task Draw(Canvas2DContext context)
        {
            await context.SetFillStyleAsync(Color);
            await context.FillRectAsync(Position.X, Position.Y, Size, Size);
        }

        public void MoveUp()
        {
            Position.Y -= MovementSpeed;
            // Adicione verificações de limite aqui se necessário
        }

        public void MoveDown()
        {
            Position.Y += MovementSpeed;
            // Adicione verificações de limite aqui se necessário
        }

        public void MoveLeft()
        {
            Position.X -= MovementSpeed;
            // Adicione verificações de limite aqui se necessário
        }

        public void MoveRight()
        {
            Position.X += MovementSpeed;
            // Adicione verificações de limite aqui se necessário
        }


        public void MoveTowardsTarget()
        {
            // Calcula a direção para a posição alvo
            var direction = new Position(TargetPosition.X - Position.X, TargetPosition.Y - Position.Y);
            var magnitude = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

            // Normaliza a direção (para ter um vetor de unidade)
            if (magnitude > 0)
            {
                direction.X /= (float)magnitude;
                direction.Y /= (float)magnitude;
            }

            // Atualiza a posição anterior para a posição atual antes de mudar a posição
            PreviousPosition = new Position(Position.X, Position.Y);

            // Move o personagem na direção do alvo
            Position.X += direction.X * MovementSpeed;
            Position.Y += direction.Y * MovementSpeed;

            // Aqui você pode adicionar condições para verificar se o personagem atingiu o alvo
            // e parar ou ajustar o movimento conforme necessário.
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "ArrowUp":
                    this.MoveUp();
                    break;
                case "ArrowDown":
                    this.MoveDown();
                    break;
                case "ArrowLeft":
                    this.MoveLeft();
                    break;
                case "ArrowRight":
                    this.MoveRight();
                    break;
            }
        }
    }

    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
