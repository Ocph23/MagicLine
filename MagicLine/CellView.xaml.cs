
using MagicLineLib;

namespace MagicLine;


public delegate Task ClickCell(CellBall cell);

public partial class CellView : ContentView, IDisposable
{

    private CellBall cell;

    public CellBall Cell
    {
        get { return cell; }
        set
        {
            cell = value;
            OnPropertyChanged(nameof(Cell));
        }
    }

    public bool IsSelected { get; private set; }

    public event ClickCell OnClickCell;

    CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
    private object animation;

    public CellView(CellBall cell)
    {
        InitializeComponent();
        this.Cell = cell;
        BindingContext = this;

    }



    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var cellView = ((StackLayout)sender).Parent as CellView;
        var cell = (CellBall)cellView.Cell;
        if (OnClickCell != null)
        {
            OnClickCell?.Invoke(cell);
        }

    }

    internal async Task Selected()
    {
        IsSelected = !IsSelected;
        if (IsSelected)
        {
            new Animation {  { 0, 0.2, new Animation (v => this.ball.Scale = v, 1, 0.8) },
                { 0.2, 0.4, new Animation (v => this.ball.Scale = v, 0.8, 1) }}
            .Commit(this, $"SelectAnimation{this.Cell.Position.Row}{this.Cell.Position.Column}", 16, 500, null, null, () => true);
        }
        else
        {
            this.AbortAnimation($"SelectAnimation{this.Cell.Position.Row}{this.Cell.Position.Column}");
        }
    }

    public void Dispose()
    {
        cancelTokenSource.Dispose();
    }
}