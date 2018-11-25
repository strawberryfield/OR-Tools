// COPYRIGHT 2018 Roberto Ceccarelli - Casasoft.
//
// Original work is COPYRIGHT 2011, 2012, 2013 by the Open Rails project.
// 
// This file is part of OR Tools.
// 
// OR Tools is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// OR Tools is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OR Tools.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.IO;
using GNU.Gettext;
using MSTS;
using Orts.Formats.Msts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ORTS.Menu
{
    public class Route
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string Path;
        public readonly string Image;
        public readonly Texture2D Texture;
        GettextResourceManager catalog = new GettextResourceManager("ORTS.Menu");

        Route(string path, Game game)
        {
            if (Directory.Exists(path))
            {
				var trkFilePath = MSTSPath.GetTRKFileName(path);
                try
                {
					var trkFile = new RouteFile(trkFilePath);
                    Name = trkFile.Tr_RouteFile.Name.Trim();
                    Description = trkFile.Tr_RouteFile.Description.Trim();
                    Image = trkFile.Tr_RouteFile.LoadingScreen.Trim();
                    
                }
                catch
                {
                    Name = "<" + catalog.GetString("load error:") + " " + System.IO.Path.GetFileName(path) + ">";
                }
                if (string.IsNullOrEmpty(Name)) Name = "<" + catalog.GetString("unnamed:") + " " + System.IO.Path.GetFileNameWithoutExtension(path) + ">";
                if (string.IsNullOrEmpty(Description)) Description = null;
                if (string.IsNullOrEmpty(Image)) Image = "load.ace";

                string imagePath = System.IO.Path.Combine(path, this.Image);
                if(File.Exists(imagePath))
                {
                    Texture = Orts.Formats.Msts.AceFile.Texture2DFromFile(game.GraphicsDevice, imagePath);
                }
                else
                {
                    Texture = null;
                }
            }
            else
            {
                Name = "<" + catalog.GetString("missing:") + " " + System.IO.Path.GetFileName(path) + ">";
            }
            Path = path;
        }

        public override string ToString()
        {
            return Name;
        }

        public static List<Route> GetRoutes(Folder folder, Game game)
        {
            var routes = new List<Route>();
            var directory = System.IO.Path.Combine(folder.Path, "ROUTES");
            if (Directory.Exists(directory))
            {
                foreach (var routeDirectory in Directory.GetDirectories(directory))
                {
                    try
                    {
                        routes.Add(new Route(routeDirectory, game));
                    }
                    catch { }
                }
            }
            return routes;
        }
    }
}
