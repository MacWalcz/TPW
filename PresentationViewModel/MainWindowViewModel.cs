//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            WindowSizeChanged += ModelLayer.OnWindowSizeChanged;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
        }


        #endregion ctor

        #region public API

        public double BorderWidth => ModelLayer.BoardWidth;
        public double BorderHeight => ModelLayer.BoardHeight;

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
        }
       
        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();
        public event Action<double, double> WindowSizeChanged;
        
        
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                if (_windowWidth != value)
                {
                    _windowWidth = value;
                    
                    WindowSizeChanged?.Invoke(WindowWidth, WindowHeight);
                    RaisePropertyChanged(nameof(BorderWidth));
                    RaisePropertyChanged(nameof(BorderHeight));
                }
            }
        }

        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                if (_windowHeight != value)
                {
                    _windowHeight = value;
                   
                    WindowSizeChanged?.Invoke(WindowWidth, WindowHeight);
                    RaisePropertyChanged(nameof(BorderWidth));
                    RaisePropertyChanged(nameof(BorderHeight));
                }
            }
        }

      
   

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
              
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;
        private double _windowWidth = 800;
        private double _windowHeight = 600;


       
        #endregion private
       

        
    
      }
    }
