#if RIVE_USING_GRAPHICS_TEST_FRAMEWORK

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Rive.Tests.Utils;
using System;
using System.Linq;
using Rive.Components;
using System.Collections.Generic;

namespace Rive.Tests
{
    public class TestPrefabReferences
    {

        public const string RivePanelWithSingleWidget = "Packages/app.rive.rive-unity.tests/PlayModeTests/Components/Goldens/TestPanels/RivePanelWithSingleWidget.prefab";
        public const string RivePanelWithMultipleWidgets = "Packages/app.rive.rive-unity.tests/PlayModeTests/Components/Goldens/TestPanels/RivePanelWithMultipleWidgets.prefab";
        public const string RivePanelWithSingleWidgetAndLayout = "Packages/app.rive.rive-unity.tests/PlayModeTests/Components/Goldens/TestPanels/RivePanelWithSingleWidgetAndLayout.prefab";
        public const string RivePanelWithProceduralWidget = "Packages/app.rive.rive-unity.tests/PlayModeTests/Components/Goldens/TestPanels/RivePanelWithProceduralWidget.prefab";

        public const string RivePanelWithInitialFrameInputs = "Packages/app.rive.rive-unity.tests/PlayModeTests/Components/Goldens/TestPanels/RivePanelWithInitialFrameInputs.prefab";
    }

    public class RivePanelGoldenTests
    {
        private class CustomTestCases
        {
            public const string AtlasStrategy_Renders_MultiplePanels = "AtlasStrategy_Renders_MultiplePanels";
        }

        public class PanelScenario
        {
            public string GoldenId { get; }
            public string PanelPrefabPath { get; }
            public Action<RivePanel> ConfigurePanel { get; }
            public IEnumerator WaitCondition { get; }

            public bool IncludeInTransformChangeTests { get; set; } = false;

            public PanelScenario(
                string goldenId,
                string panelPrefabPath,
                Action<RivePanel> configurePanel = null,
                IEnumerator waitCondition = null,
                bool includeInTransformChangeTests = false)
            {
                GoldenId = goldenId;
                PanelPrefabPath = panelPrefabPath;
                ConfigurePanel = configurePanel ?? (_ => { });
                WaitCondition = waitCondition;
                IncludeInTransformChangeTests = includeInTransformChangeTests;
            }

            public static IEnumerator WaitForSeconds(float seconds)
            {
                yield return new UnityEngine.WaitForSeconds(seconds);
            }

            public static IEnumerator WaitAFrame()
            {
                yield return null;
            }
        }

        private GoldenTestHelper m_goldenHelper;
        private TestAssetLoadingManager m_testAssetLoadingManager;

        private Camera m_camera;

        private PanelScenario[] GetTestCases()
        {
            return new[]
        {
            new PanelScenario(
                goldenId: "RivePanelWithMultipleWidgets",
                panelPrefabPath: TestPrefabReferences.RivePanelWithMultipleWidgets,
                includeInTransformChangeTests: true
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Contain;
                },
                includeInTransformChangeTests: true

            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Fill",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Fill;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Cover",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Cover;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_FitWidth",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.FitWidth;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_FitHeight",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.FitHeight;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_None",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.None;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_ScaleDown",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                }
            ),

            // Alignment
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_TopLeft",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.TopLeft;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_TopCenter",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.TopCenter;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_TopRight",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.TopRight;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_CenterLeft",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.CenterLeft;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_CenterCenter",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.Center;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_CenterRight",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.CenterRight;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_BottomLeft",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.BottomLeft;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_BottomCenter",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.BottomCenter;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidget_Artboard_FitMode_Contain_Alignment_BottomRight",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidget,
                configurePanel: (panel) => {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.ScaleDown;
                    widget.Alignment = Alignment.BottomRight;
                }
            ),


            // Procedural widget
            new PanelScenario(
                goldenId: "RivePanelWithProceduralWidget",
                panelPrefabPath: TestPrefabReferences.RivePanelWithProceduralWidget,
                includeInTransformChangeTests: false
            ),

            // Layout scaling modes

            //ReferenceArtboardSize
            // The widget will look the same at different resolutions, as long as the aspect ratio is the same.
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ReferenceArtboardSize_Landscape_1920x1080",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1920, 1080));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ReferenceArtboardSize;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ReferenceArtboardSize_Portrait_1080x1920",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1080, 1920));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ReferenceArtboardSize;
                }
            ),
             new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ReferenceArtboardSize_Landscape_1280x720",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1280, 720));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ReferenceArtboardSize;
                }
            ),

            // ConstantPixelSize 
            // The widget will look smaller on a larger resolution screen, and larger on a smaller resolution screen.
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ReferenceArtboardSize_Landscape_1920x1080_ScaleFactor_2",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1920, 1080));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ReferenceArtboardSize;
                    widget.ScaleFactor = 2f;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ConstantPixelSize_Landscape_1920x1080",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1920, 1080));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ConstantPixelSize;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ConstantPixelSize_Portrait_1080x1920",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1080, 1920));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ConstantPixelSize;
                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ConstantPixelSize_Landscape_1280x720",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1280, 720));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ConstantPixelSize;
                }
            ),            
            // Test that the layout is affected by dpi:
            // For example, higher DPR device it appear zoomed in, while lower DPR device it appear zoomed out.
             new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ConstantPhysicalSize_Landscape_1920x1080_HighDPI",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1920, 1080));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.UseFallbackDPI = true;
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ConstantPhysicalSize;
                    widget.FallbackDPI = 225f;
                    widget.ReferenceDPI = 150f;


                }
            ),
            new PanelScenario(
                goldenId: "RivePanelWithSingleWidgetAndLayout_FitMode_Layout_ConstantPhysicalSize_Landscape__1920x1080_LowDPI",
                panelPrefabPath: TestPrefabReferences.RivePanelWithSingleWidgetAndLayout,
                configurePanel: (panel) => {
                    panel.SetDimensions(new Vector2(1920, 1080));
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.UseFallbackDPI = true;
                    widget.Fit = Fit.Layout;
                    widget.ScalingMode = LayoutScalingMode.ConstantPhysicalSize;
                    widget.FallbackDPI = 96f;
                    widget.ReferenceDPI = 150f;


                }
            ),
        };
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            m_testAssetLoadingManager = new TestAssetLoadingManager();

            // Important: Make sure the resolution is set to a number with a power of 2 (e.g. 512, 1024, 2048, etc.) otherwise, unity will resize the saved image
            // - Make sure the images are saved as and set to RGBA32 in the Unity editor
            // - Make sure the images have Read/Write enabled in the Unity editor
            // - Set captureGolden to true to generate golden images.
            m_goldenHelper = new GoldenTestHelper(assetLoadingManager: m_testAssetLoadingManager, referenceImagesPath: "Packages/app.rive.rive-unity.tests/PlayModeTests/Components/Goldens/ReferenceImages", captureGolden: false, maxResolution: 512, savedImageFormat: GoldenTestHelper.SavedImageFormatType.PNG);

        }

        [SetUp]
        public void Setup()
        {
            m_camera = new GameObject("Camera").AddComponent<Camera>();
            m_camera.tag = "MainCamera";

        }

        [TearDown]
        public void TearDown()
        {
            m_goldenHelper.Cleanup();
            m_testAssetLoadingManager.UnloadAllAssets();
            DestroyObj(m_camera.gameObject);

        }


        private IEnumerator SetupTestPanel(PanelScenario testCase)
        {
            RivePanel panel = null;
            yield return m_testAssetLoadingManager.LoadAssetCoroutine<GameObject>(
                testCase.PanelPrefabPath,
                (prefab) =>
                {
                    var panelObj = UnityEngine.Object.Instantiate(prefab);
                    panel = panelObj.GetComponent<RivePanel>();
                },
                () => Assert.Fail($"Failed to load panel prefab at {testCase.PanelPrefabPath}")
            );

            testCase.ConfigurePanel(panel);
            yield return panel;
        }

        [UnityTest]
        public IEnumerator Panel_RendersOnFirstFrame()
        {
            foreach (var testCase in GetTestCases())
            {
                var setupResult = SetupTestPanel(testCase);
                yield return setupResult;
                var panel = (RivePanel)setupResult.Current;

                // Wait for end of frame to ensure rendering has occurred.
                yield return new WaitForEndOfFrame();

                Assert.IsNotNull(panel.RenderTexture, "RenderTexture should be created on first frame");
                Assert.IsTrue(panel.RenderTexture.width > 0 && panel.RenderTexture.height > 0,
                    "RenderTexture should have valid dimensions");

                yield return m_goldenHelper.AssertWithRenderTexture(
                    testCase.GoldenId,
                    panel.RenderTexture
                );

                DestroyObj(panel.gameObject);
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Panel_TransformChanges_MaintainsRenderTexture()
        {
            var transformTests = new (string name, Action<Transform> action)[]
            {
                // We check that the render texture remains the same after each transform change because if the RectTransform to RenderTransform conversion is working correctly, the positions of the RiveWidgets within the panel should not be affected by rotation, scale, or position changes.
                // Depending on how the conversion is done, if handled incorrectly, rotating the panel may cause widget positions to shift.
                (name: "RotateX", action: (Transform t) => t.rotation = Quaternion.Euler(45, 0, 0)),
                (name: "RotateY", action: (Transform t) => t.rotation = Quaternion.Euler(0, 45, 0)),
                (name: "RotateZ", action: (Transform t) => t.rotation = Quaternion.Euler(0, 0, 45)),
                (name: "RotateAll", action: (Transform t) => t.rotation = Quaternion.Euler(30, 45, 60)),
                (name: "Scale", action: (Transform t) => t.localScale = new Vector3(3f, 3f, 3f)),
                (name: "Position", action: (Transform t) => t.position = new Vector3(100, -50, 25))
            };

            // Only test the panel scenarios where IncludeInTransformChangeTests is true
            PanelScenario[] transformTestCases = GetTestCases().Where(testCase => testCase.IncludeInTransformChangeTests).ToArray();

            Assert.Greater(transformTestCases.Length, 0, "No test cases found with IncludeInTransformChangeTests set to true");



            foreach (var testCase in transformTestCases)
            {
                var setupResult = SetupTestPanel(testCase);
                yield return setupResult;
                var panel = (RivePanel)setupResult.Current;

                yield return new WaitForEndOfFrame();
                if (testCase.WaitCondition != null)
                {
                    yield return testCase.WaitCondition;
                }

                // Capture initial render
                yield return m_goldenHelper.AssertWithRenderTexture(
                    testCase.GoldenId,
                    panel.RenderTexture
                );

                // Test each transform change
                foreach (var test in transformTests)
                {
                    test.action(panel.transform);

                    // The render texture should remain unchanged
                    yield return m_goldenHelper.AssertWithRenderTexture(
                        testCase.GoldenId,
                        panel.RenderTexture
                    );
                }

                DestroyObj(panel.gameObject);
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator AtlasStrategy_MultiplePanels_ShareSingleRenderTexture()
        {
            // Create and configure the atlas strategy
            var atlasStrategy = new GameObject("AtlasStrategy").AddComponent<AtlasRenderTargetStrategy>();
            atlasStrategy.Configure(
                startingSize: new Vector2Int(1048, 1024),
                maxAtlasSize: new Vector2Int(1048, 1024),
                maxResolutionPerPanel: 512,
                padding: 2
            );

            var panels = new List<RivePanel>();

            var testCases = GetTestCases().Take(3).ToArray();

            // Create multiple panels using our test cases
            foreach (var testCase in testCases)
            {
                var setupResult = SetupTestPanel(testCase);
                yield return setupResult;
                var panel = (RivePanel)setupResult.Current;

                // Assign the atlas strategy to each panel
                panel.RenderTargetStrategy = atlasStrategy;
                panels.Add(panel);

                if (testCase.WaitCondition != null)
                {
                    yield return testCase.WaitCondition;
                }

                panel.StartRendering();

                Assert.IsTrue(panel.IsRendering, "Panel should be rendering");
            }

            yield return null;

            // Wait for all panels to render
            yield return new WaitForEndOfFrame();

            // Verify that all panels are using the same render texture (the atlas)
            var atlasTexture = panels[0].RenderTexture;
            Assert.IsNotNull(atlasTexture, "Atlas texture should be created");

            foreach (var panel in panels)
            {
                Assert.AreEqual(atlasTexture, panel.RenderTexture,
                    "All panels should share the same atlas texture");
            }

            // Verify that the atlas contains all panel renders
            yield return m_goldenHelper.AssertWithRenderTexture(
                CustomTestCases.AtlasStrategy_Renders_MultiplePanels,
                atlasTexture
            );

            // Cleanup
            foreach (var panel in panels)
            {
                DestroyObj(panel.gameObject);
            }
            DestroyObj(atlasStrategy.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PooledStrategy_HandlesMultipleAspectRatios()
        {
            int maxPoolSize = 3;
            var pooledStrategy = new GameObject("PooledStrategy").AddComponent<PooledRenderTargetStrategy>();
            pooledStrategy.Configure(
                textureSize: new Vector2Int(512, 512),
                initialPoolSize: maxPoolSize,
                maxPoolSize: maxPoolSize,
                PooledRenderTargetStrategy.PoolOverflowBehavior.Fixed
            );

            var panels = new List<RivePanel>();

            // Test cases with different aspect ratios
            var aspectRatioTests = new (string prefabPath, Vector2 dimensions, string goldenId)[]
            {
                (TestPrefabReferences.RivePanelWithSingleWidget, new Vector2(1920, 1080), $"PooledStrategy_HandlesMultipleAspectRatios_{nameof(TestPrefabReferences.RivePanelWithSingleWidget)}_1920x1080"),
                (TestPrefabReferences.RivePanelWithSingleWidget, new Vector2(1080, 1920), $"PooledStrategy_HandlesMultipleAspectRatios_{nameof(TestPrefabReferences.RivePanelWithSingleWidget)}_1080x1920"),
                (TestPrefabReferences.RivePanelWithMultipleWidgets, new Vector2(1000, 1000), $"PooledStrategy_HandlesMultipleAspectRatios_{nameof(TestPrefabReferences.RivePanelWithMultipleWidgets)}_1000x1000"),
            };

            // We create panels with different aspect ratios
            foreach (var test in aspectRatioTests)
            {
                var setupResult = SetupTestPanel(new PanelScenario(
                    goldenId: test.goldenId,
                    panelPrefabPath: test.prefabPath,
                    configurePanel: (panel) =>
                    {
                        panel.SetDimensions(test.dimensions);
                        panel.RenderTargetStrategy = pooledStrategy;

                        if (test.prefabPath == TestPrefabReferences.RivePanelWithSingleWidget)
                        {
                            // Get the widget and set the fit mode to fill so we can see the effect of the different aspect ratios
                            var widget = panel.GetComponentInChildren<RiveWidget>();
                            widget.Fit = Fit.Fill;
                        }


                    }
                ));

                yield return setupResult;
                var panel = (RivePanel)setupResult.Current;
                panels.Add(panel);

                panel.StartRendering();
                Assert.IsTrue(panel.IsRendering, "Panel should be rendering");
            }

            yield return new WaitForEndOfFrame();

            // Verify all render textures are 512x512 (maxResolutionPerPanel)
            for (int i = 0; i < panels.Count; i++)
            {
                var panel = panels[i];
                Assert.IsNotNull(panel.RenderTexture, "Panel should have a render texture");
                Assert.AreEqual(512, panel.RenderTexture.width, "Render texture width should be 512");
                Assert.AreEqual(512, panel.RenderTexture.height, "Render texture height should be 512");

                yield return m_goldenHelper.AssertWithRenderTexture(
                    aspectRatioTests[i].goldenId,
                    panel.RenderTexture
                );

            }

            // Verify we're reusing textures from the pool (should have 3 or fewer unique textures)
            var uniqueTextures = panels
                .Select(p => p.RenderTexture)
                .Distinct()
                .Count();
            Assert.LessOrEqual(uniqueTextures, maxPoolSize, "Should not create more textures than pool size");

            // Cleanup
            foreach (var panel in panels)
            {
                DestroyObj(panel.gameObject);
            }
            DestroyObj(pooledStrategy.gameObject);
            yield return null;
        }

        /// <summary>
        /// Tests that setting inputs on the initial frame (in the OnLoad callback) affects the visuals
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator PanelWithInitialFrameInputs_AffectsVisuals()
        {
            // This example fires the "Ready" trigger, which displays the 'Ready' text on the artboard
            // It should be visible in the golden image

            var setupResult = SetupTestPanel(new PanelScenario(
                goldenId: "RivePanel_WithInitialFrameInputs_FireTrigger_AffectsVisuals",
                panelPrefabPath: TestPrefabReferences.RivePanelWithInitialFrameInputs,
                configurePanel: (panel) =>
                {
                    var widget = panel.GetComponentInChildren<RiveWidget>();
                    widget.OnWidgetStatusChanged += () =>
                    {
                        if (widget.Status == WidgetStatus.Loaded)
                        {
                            widget.StateMachine.GetTrigger("Ready")?.Fire();
                        }
                    };
                }
            ));
            yield return setupResult;
            var panel = (RivePanel)setupResult.Current;

            yield return new WaitForEndOfFrame();

            // Verify that the initial frame inputs have affected the visuals
            yield return m_goldenHelper.AssertWithRenderTexture(
                "RivePanel_WithInitialFrameInputs_FireTrigger_AffectsVisuals",
                panel.RenderTexture
            );

            DestroyObj(panel.gameObject);
            yield return null;
        }

        private void DestroyObj(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
    }
}
#endif