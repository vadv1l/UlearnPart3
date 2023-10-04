using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FractalPainting.App.Fractals;
using FractalPainting.Infrastructure.Common;
using FractalPainting.Infrastructure.UiActions;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Conventions;

namespace FractalPainting.App
{
    public static class DIContainerTask
    {
        public static MainForm CreateMainForm() => ConfigureContainer().Get<MainForm>();

        public static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind(bindingObject => bindingObject
                .FromThisAssembly().SelectAllClasses()
                .InheritedFrom<IUiAction>().BindAllInterfaces());
            container.Bind<IObjectSerializer, XmlObjectSerializer>().To<XmlObjectSerializer>();
            container.Bind<IBlobStorage, FileBlobStorage>().To<FileBlobStorage>();
            container.Bind<AppSettings>().ToMethod(context => context.Kernel
                .Get<SettingsManager>().Load()).InSingletonScope();
            container.Bind<ImageSettings>()
                .ToMethod(context => context.Kernel.Get<SettingsManager>()
                    .Load().ImageSettings).InSingletonScope();
            container.Bind<IImageHolder, PictureBoxImageHolder>().To<PictureBoxImageHolder>().InSingletonScope();
            container.Bind<Palette>().ToSelf().InSingletonScope();
            container.Bind<IDragonPainterFactory>().ToFactory();
            return container;
        }
    }

    public class DragonFractalAction : IUiAction
    {
        private IDragonPainterFactory _dragonPainterFactory;
        public DragonFractalAction(IDragonPainterFactory dragonPainterFactory) => 
            _dragonPainterFactory = dragonPainterFactory;
        public MenuCategory Category => MenuCategory.Fractals;
        public string Name => "Дракон";
        public string Description => "Дракон Хартера-Хейтуэя";

        public void Perform()
        {
            var dragonSettings = CreateRandomSettings();
            SettingsForm.For(dragonSettings).ShowDialog();
            _dragonPainterFactory.CreatePainter(dragonSettings).Paint();
        }

        private static DragonSettings CreateRandomSettings() => 
            new DragonSettingsGenerator(new Random()).Generate();
    }

    public interface IDragonPainterFactory
    {
        DragonPainter CreatePainter(DragonSettings dragonSettings);
    }

    public class KochFractalAction : IUiAction
    {
        private Lazy<KochPainter> _kochPainter;

        public KochFractalAction(Lazy<KochPainter> kochPainter) => _kochPainter = kochPainter; 

        public MenuCategory Category => MenuCategory.Fractals;
        public string Name => "Кривая Коха";
        public string Description => "Кривая Коха";

        public void Perform() => _kochPainter.Value.Paint();
    }

    public class DragonPainter
    {
        private readonly IImageHolder _imageHolder;
        private readonly DragonSettings _settings;
        private Palette _palette;

        public DragonPainter(IImageHolder imageHolder, DragonSettings settings, Palette palette)
        {
            _imageHolder = imageHolder;
            _settings = settings;
            _palette = palette;
        }

        public void Paint()
        {
            using (var graphics = _imageHolder.StartDrawing())
            {
                var imageSize = _imageHolder.GetImageSize();
                var size = GetFloatSize(imageSize);
                graphics.FillBackGround(_palette, imageSize);
                var r = new Random();
                var aCosSin = GetCosSinPair(_settings.Angle1);
                var bCosSin = GetCosSinPair(_settings.Angle2);
                var shiftX = _settings.ShiftX * size * 0.8f;
                var shiftY = _settings.ShiftY * size * 0.8f;
                var scale = _settings.Scale;
                var p = new PointF(0, 0);
                DrawDragon(graphics, imageSize, aCosSin, 
                    bCosSin, p, scale, shiftX, shiftY, r);
            }
            _imageHolder.UpdateUi();
        }

        private void DrawDragon(Graphics graphics, Size imageSize,
            IReadOnlyList<float> aCosSin, IReadOnlyList<float> bCosSin, 
            PointF p, float scale, float shiftX, float shiftY, Random r)
        {
            foreach (var i in Enumerable.Range(0, _settings.IterationsCount))
            {
                using (var brush = new SolidBrush(_palette.PrimaryColor))
                {
                    graphics.FillRectangle(brush, imageSize.Width / 3f + p.X, imageSize.Height / 2f + p.Y,
                        1, 1);
                    if (r.Next(0, 2) == 0)
                        p = new PointF(scale * (p.X * aCosSin[0] - p.Y * aCosSin[1]),
                            scale * (p.X * aCosSin[1] + p.Y * aCosSin[0]));
                    else
                        p = new PointF(scale * (p.X * bCosSin[0] - p.Y * bCosSin[1]) + shiftX,
                            scale * (p.X * bCosSin[1] + p.Y * bCosSin[0]) + shiftY);
                    if (i % 100 == 0) _imageHolder.UpdateUi();
                }
            }
        }

        private static float[] GetCosSinPair(double angle) =>
            new[] { (float)Math.Cos(angle), (float)Math.Sin(angle) };

        private static float GetFloatSize(Size imageSize) =>
            Math.Min(imageSize.Width, imageSize.Height) / 2.1f;
    }

    public static class GraphicsExtension
    {
        public static void FillBackGround(this Graphics graphics, Palette palette, Size imageSize)
        {
            using (var brush = new SolidBrush(palette.BackgroundColor))
                graphics.FillRectangle(brush, 0, 0, imageSize.Width, imageSize.Height);
        }
    }
}