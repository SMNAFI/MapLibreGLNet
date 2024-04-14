using CefSharp;
using CefSharp.DevTools.LayerTree;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MapLibreGL
{
    public partial class WebMap : UserControl
    {
        CefSharp.WinForms.ChromiumWebBrowser webView;
        double centerX, centerY;

        public WebMap()
        {
            InitializeComponent();

            if (!Cef.IsInitialized)
            {
                CefSettings settings = new CefSettings();

                Cef.Initialize(settings);

                initializeMap();
            }
        }

        void initializeMap()
        {
            if (webView != null)
            {
                this.Controls.Remove(webView);
                webView = null;
            }

            BrowserSettings browserSettings = new BrowserSettings();

            webView = new ChromiumWebBrowser();
            webView.IsBrowserInitializedChanged += WebView_IsBrowserInitializedChanged;
            webView.FrameLoadEnd += WebView_FrameLoadEnd;
            webView.BrowserSettings = browserSettings;

            this.Controls.Add(webView);
            webView.Dock = DockStyle.Fill;
        }
        private void WebView_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            webView.LoadUrl($@"{Application.StartupPath}\Map\WebMap.html");
            webView.ShowDevTools();
        }

        private void WebView_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            LoadGeoJsonDataSources();
            CreateMapLayers();
            UpdateMapCenter();
        }

        private void LoadGeoJsonDataSources()
        {
            string extentGeoJSONString = File.ReadAllText(@"C:\Users\smnaf\Downloads\obj\Extent.geojson");
            var extentGeoJSON = JObject.Parse(extentGeoJSONString);

            // Deserialize the JObject into a dynamic object
            dynamic jsonObj = JsonConvert.DeserializeObject(extentGeoJSON.ToString());
            // Access cenX and cenY properties
            centerX = jsonObj.features[0].properties.CNTX;
            centerY = jsonObj.features[0].properties.CNTY;

            webView.EvaluateScriptAsync("AddDataSource", new object[] { "extent-area", extentGeoJSON });


            string landGeoJSONString = File.ReadAllText(@"C:\Users\smnaf\Downloads\obj\LandA.geojson");
            var landGeoJSON = JObject.Parse(landGeoJSONString);
            webView.EvaluateScriptAsync("AddDataSource", new object[] { "land-area", landGeoJSON });


            string waterGeoJSONString = File.ReadAllText(@"C:\Users\smnaf\Downloads\obj\WaterA.geojson");
            var waterGeoJSON = JObject.Parse(waterGeoJSONString);
            webView.EvaluateScriptAsync("AddDataSource", new object[] { "water-area", waterGeoJSON });


            string buildingGeoJSONString = File.ReadAllText(@"C:\Users\smnaf\Downloads\obj\BuildingA.geojson");
            var buildingGeoJSON = JObject.Parse(buildingGeoJSONString);
            webView.EvaluateScriptAsync("AddDataSource", new object[] { "building-area", buildingGeoJSON });
        }

        private void CreateMapLayers()
        {
            CreateExtentLayer();
            CreateLandLayer();
            CreateWaterLayer();
            CreateBuildingLayer();

            //webView.EvaluateScriptAsync("AddLayers()");
        }

        private void CreateBuildingLayer()
        {
            var obj = new Dictionary<string, object>
            {
                ["id"] = "building-area-layer",
                ["type"] = "fill-extrusion",
                ["source"] = "building-area",
                ["paint"] = new Dictionary<string, object>
                {
                    ["fill-extrusion-color"] = new object[] {
                                                    "get",
                                                    "color",
                                                },
                    ["fill-extrusion-height"] = new object[] {
                                                "get",
                                                "height",
                                            },
                    ["fill-extrusion-base"] = new object[] {
                                                "get",
                                                "baseheight",
                                            },
                    ["fill-extrusion-opacity"] = 0.9,
                },
                ["layout"] = new Dictionary<string, object>
                {
                    ["visibility"] = "visible",
                },
            };

            var json = JsonConvert.SerializeObject(obj);
            webView.EvaluateScriptAsync("AddLayer", json);
        }

        private void CreateWaterLayer()
        {
            var obj = new Dictionary<string, object>
            {
                ["id"] = "water-area-layer",
                ["type"] = "fill-extrusion",
                ["source"] = "water-area",
                ["paint"] = new Dictionary<string, object>
                {
                    ["fill-extrusion-color"] = "blue",
                    ["fill-extrusion-height"] = 0.1,
                    ["fill-extrusion-base"] = 0.1,
                    ["fill-extrusion-opacity"] = 1,
                },
            };

            var json = JsonConvert.SerializeObject(obj);
            webView.EvaluateScriptAsync("AddLayer", json);
        }

        private void CreateLandLayer()
        {
            var obj = new Dictionary<string, object>
            {
                ["id"] = "land-area-layer",
                ["type"] = "fill-extrusion",
                ["source"] = "land-area",
                ["paint"] = new Dictionary<string, object>
                {
                    ["fill-extrusion-color"] = "#d3dfbc",
                    ["fill-extrusion-height"] = 0.1,
                    ["fill-extrusion-base"] = 0.1,
                    ["fill-extrusion-opacity"] = 1
                }
            };

            var json = JsonConvert.SerializeObject(obj);
            webView.EvaluateScriptAsync("AddLayer", json);
        }

        private void CreateExtentLayer()
        {
            var obj = new Dictionary<string, object>
            {
                ["id"] = "extent-area-layer",
                ["type"] = "fill-extrusion",
                ["source"] = "extent-area",
                ["paint"] = new Dictionary<string, object>
                {
                    ["fill-extrusion-color"] = new object[] {
                                                    "get",
                                                    "color",
                                                },
                    ["fill-extrusion-height"] = 0,
                    ["fill-extrusion-base"] = 0,
                    ["fill-extrusion-opacity"] = 1,
                },
            };

            var json = JsonConvert.SerializeObject(obj);
            webView.EvaluateScriptAsync("AddLayer", json);
        }

        private void UpdateMapCenter()
        {
            webView.EvaluateScriptAsync("UpdateCenter", new object[] { 27.981720, 7.719564 });
            //webView.EvaluateScriptAsync("UpdateCenter", new object[] { centerX, centerY });
        }
    }
}
