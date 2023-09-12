
namespace MagicLine;


public delegate Task ClickCell(MagicLineLib.Cell cell);

public partial class CellView : ContentView ,IDisposable
{

    private MagicLineLib.Cell cell;

    public MagicLineLib.Cell Cell
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
    public CellView(MagicLineLib.Cell cell)
    {
        InitializeComponent();
        this.Cell = cell;
        BindingContext = this;
       
    }

    private async Task StartAnimation(CancellationToken token)
    {
        while (IsSelected)
        {
            if(token.IsCancellationRequested)
                IsSelected = false;
            await this.ball.ScaleTo(0.5, 250);
            await this.ball.ScaleTo(1, 250);
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var cellView = ((StackLayout)sender).Parent as CellView;
        var cell = (MagicLineLib.Cell)cellView.Cell;
        if (OnClickCell != null)
        {
            OnClickCell?.Invoke(cell);
        }

    }

    internal async Task Selected()
    {
        IsSelected=!IsSelected;
        if(IsSelected)
          await  StartAnimation(cancelTokenSource.Token);
        else
        {
            cancelTokenSource.Cancel();
        }

    }

    public void Dispose()
    {
        cancelTokenSource.Dispose();
    }
}