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
            Observer = ModelLayer.Subscribe<ModelIBall>(x =>
            {
        
                Balls.Add(x);
            });  
        }
         

        #endregion ctor

        #region public API

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
        }

<<<<<<< HEAD
        public event Action<double, double> ScaleChanged;

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                if (_windowWidth != value)
                {
                    _windowWidth = value;
                    RaisePropertyChanged(nameof(WindowWidth));
                    RaisePropertyChanged(nameof(ScaleWidth));
                    UpdateBorderSize();
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
                    RaisePropertyChanged(nameof(WindowHeight));
                    RaisePropertyChanged(nameof(ScaleHeight));
                    UpdateBorderSize();
                }
            }
        }

        public double ScaleWidth => WindowWidth > 0 ? WindowWidth / 1000 : 1;
        public double ScaleHeight => WindowHeight > 0 ? WindowHeight / 1000 : 1;
        public double BorderWidth => ScaleWidth * ModelAbstractApi.PresentationDimensions.TableWidth * 4; 
        public double BorderHeight => ScaleHeight * ModelAbstractApi.PresentationDimensions.TableHeight * 4.2;

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    foreach (ModelIBall ball in Balls)
                    {
                        ScaleChanged -= ball.UpdateScale;
                    }
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


        private void UpdateBorderSize()
        {
            RaisePropertyChanged(nameof(BorderWidth));
            RaisePropertyChanged(nameof(BorderHeight));
          

        }
        #endregion private
       
=======
        
        Disposed = true;
      }
>>>>>>> 2c9cce15e58a29fa485b6995f234ffa9e6fa81f0
    }
}