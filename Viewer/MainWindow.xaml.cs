﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

namespace Wpf3DTest {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
    public partial class MainWindow : Window
    {
        private bool _isMouseDown;
        private Point _lastMousePosition;
        private Model3DGroup _modelGeometry;

        private RotateTransform3D _initialModelTransform = new RotateTransform3D(new QuaternionRotation3D(new Quaternion(new Vector3D(1, 0, 0), -90)));

        public MainWindow()
        {
            InitializeComponent();
            AttachModel();
            InitialiseScene();
        }

        private void InitialiseScene()
        {
            _modelGeometry.Transform = new Transform3DGroup();
            ((Transform3DGroup)_modelGeometry.Transform).Children.Add(_initialModelTransform);
            _camera.Position = new Point3D(0, 0, 15);
        }

        private void AttachModel()
        {
            // http://helixtoolkit.codeplex.com/
            var importer = new ModelImporter();
            
            // http://animium.com/2012/06/pitts-special-aircraft-3d-model/
            string modelFile = @".\Models\Pitts Special.3ds";
            
            _modelGeometry = importer.Load(modelFile);
            _modelGeometry.Transform = new Transform3DGroup();
            _model3dGroup.Children.Add(_modelGeometry);
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _camera.Position = new Point3D(_camera.Position.X, _camera.Position.Y, _camera.Position.Z - e.Delta / 150D);
        }

        private void SetZoom(double cameraZ)
        {
            _camera.Position = new Point3D(_camera.Position.X, _camera.Position.Y, cameraZ);
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            InitialiseScene();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                Point pos = Mouse.GetPosition(viewport);
                Point mousePosition = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                double dx = mousePosition.X - _lastMousePosition.X;
                double dy = mousePosition.Y - _lastMousePosition.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    
                    if (dx < 0 && dy > 0)
                    {
                        mouseAngle += Math.PI / 2;
                    }
                    else if (dx < 0 && dy < 0)
                    {
                        mouseAngle += Math.PI;
                    }
                    else if (dx > 0 && dy < 0)
                    {
                        mouseAngle += Math.PI * 1.5;
                    }
                }
                else if (dx == 0 && dy != 0)
                {
                    mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                }
                else if (dx != 0 && dy == 0)
                {
                    mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;
                }

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
                ((Transform3DGroup)_modelGeometry.Transform).Children.Add(new RotateTransform3D(r));
                
                _lastMousePosition = mousePosition;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            _isMouseDown = true;
            Point pos = Mouse.GetPosition(viewport);
            _lastMousePosition = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }
    }
}
