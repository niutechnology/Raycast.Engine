namespace BlazorApp1.Components.Pages;

public partial class Home
{
    private void StartAnimation()
    {
        _timer = new System.Timers.Timer(20);
        _timer.Elapsed += async (_, __) => await UpdateAndRedrawCharacter();
        _timer.Start();
    }

    private async Task UpdateAndRedrawCharacter()
    {
        _character.Move(_canvasReference.Width);
        await RedrawCanvas();
    }

    private async Task RedrawCanvas()
    {
        // Limpa apenas a área anterior do personagem para evitar apagar o mapa inteiro
        var previousPosition = _character.PreviousPosition;
        var size = _character.Size;
        await _context.ClearRectAsync(previousPosition.X, previousPosition.Y, size, size);

        // Redesenhe o mapa se necessário. Se o mapa não muda, você não precisa redesenhá-lo
        // toda vez, apenas quando o personagem se move sobre ele, para limpar a trilha antiga.
        // RedrawMap(); // Implemente esta função conforme necessário.

        // Desenhe o personagem na nova posição
        await _character.Draw(_context);

        // Guarde a posição atual do personagem como a posição anterior para a próxima atualização
        _character.UpdatePreviousPosition();
    }

    public WorldMap CreateMap(int width, int height)
    {
        var map = new WorldMap();
        map.Width = width;
        map.Height = height;
        map.Pixels = new Pixel[width][];
        for (int i = 0; i < width; i++)
        {
            map.Pixels[i] = new Pixel[height];
            for (int j = 0; j < height; j++)
            {
                try
                {
                    map.Pixels[i][j] = new Pixel() { x = i, y = j, MyMaterial = (Material)_worldMap[i][j] };
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
        return map;
    }

    public class WorldMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Pixel[][] Pixels { get; set; }
    }

    public class Pixel
    {
        public Material MyMaterial { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public static Pixel P(int x, int y, Material material)
        {
            return new Pixel() { x = x, y = y, MyMaterial = material };
        }
    }

    public enum Material
    {
        FreeSpace = 0,
        Wall = 1,
        Player = 2,
        Enemy = 3
    }

    public static class Materials
    {
        public static string[] TextureMaterials = new string[]
        {
            "#4a210d",
            "#ae2828",
            "#41b741",
            "#000000"
        };

        public static string GetTexture(Material index)
        {
            return TextureMaterials[(int)index];
        }
    }

    private async Task DrawMap()
    {
        if (this.MyMap != null)
        {
            int pixelSize = 50; // Tamanho de cada "pixel" no canvas

            Parallel.ForEach(this.MyMap.Pixels, (row, state, rowIndex) =>
            {
                for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    var pixel = row[columnIndex];
                    // Aqui você coletaria os comandos para desenhar mais tarde,
                    // porque você não pode desenhar no contexto do canvas diretamente de uma thread paralela.
                    var color = Materials.GetTexture(pixel.MyMaterial);
                    var command = new DrawCommand(color, columnIndex * pixelSize, rowIndex * pixelSize, pixelSize);
                    lock (drawCommands)
                    {
                        drawCommands.Add(command);
                    }
                }
            });

            // Aplicar os comandos de desenho no contexto do canvas
            foreach (var command in drawCommands)
            {
                await _context.SetFillStyleAsync(command.Color);

                await _context.SetFillStyleAsync(command.Color);
                await _context.FillRectAsync(command.X, command.Y, command.Size, command.Size);
            }
        }
    }


    private async Task DrawMap1()
    {
        if (this.MyMap != null)
        {
            int pixelSize = 50; // Tamanho de cada "pixel" no canvas
            var tasks = new List<Task>();

            for (int i = 0; i < this.MyMap.Pixels.Length; i++)
            {
                int row = i;
                // Enqueue a task to process each row
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < this.MyMap.Pixels[row].Length; j++)
                    {
                        var pixel = this.MyMap.Pixels[row][j];
                        // We cannot use _context directly here because it is not thread-safe
                        // Instead, we will collect the draw commands and apply them all at once later
                        var color = Materials.GetTexture(pixel.MyMaterial);
                        var command = new DrawCommand(color, j * pixelSize, row * pixelSize, pixelSize);
                        lock (drawCommands)
                        {
                            drawCommands.Add(command);
                        }
                    }
                }));
            }

            // Wait for all the row tasks to complete
            await Task.WhenAll(tasks);

            // Now we have a list of draw commands, we can apply them to the context in one go
            foreach (var command in drawCommands)
            {
                await _context.SetFillStyleAsync(command.Color);
                await _context.FillRectAsync(command.X, command.Y, command.Size, command.Size);
            }
        }
    }

    // A simple class to hold draw commands
    public class DrawCommand
    {
        public string Color { get; }
        public float X { get; }
        public float Y { get; }
        public float Size { get; }

        public DrawCommand(string color, float x, float y, float size)
        {
            Color = color;
            X = x;
            Y = y;
            Size = size;
        }
    }

    // This list will hold all the draw commands generated by the tasks
}
