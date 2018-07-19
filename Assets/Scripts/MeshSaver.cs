// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace HoloToolkit.Unity.SpatialMapping {
    /// <summary>
    /// MeshSaver is a static class containing methods used for saving and loading meshes.
    /// </summary>

    public static class MeshSaver {
        /// <summary>
        /// The extension given to mesh files.
        /// </summary>
        private static string fileExtension = ".room";
        private const string ExportDirectoryKey = "_ExportDirectory";
        private const string ExportDirectoryDefault = "MeshExport";
        private const string ExportDialogErrorTitle = "Export Error";
        private const string WavefrontFileExtension = ".obj";

        /// <summary>
        /// Read-only property which returns the folder path where mesh files are stored.
        /// </summary>
        public static string MeshFolderName {
            get {
#if !UNITY_EDITOR && UNITY_WSA
                return ApplicationData.Current.RoamingFolder.Path;
#else
                return Application.persistentDataPath;
#endif
            }
        }

        /// <summary>
        /// Transforms all the mesh vertices into world position before saving to file.
        /// </summary>
        /// <param name="fileName">Name to give the saved mesh file. Exclude path and extension.</param>
        /// <param name="meshes">The collection of Mesh objects to save.</param>
        /// <returns>Fully qualified name of the saved mesh file.</returns>
        /// <remarks>Determines the save path to use and automatically applies the file extension.</remarks>
        public static string Save(string fileName, IEnumerable<MeshFilter> meshFilters) {
            if (string.IsNullOrEmpty(fileName)) {
                throw new ArgumentException("Must specify a valid fileName.");
            }

            if (meshFilters == null) {
                throw new ArgumentNullException("Value of meshFilters cannot be null.");
            }

            // Create the mesh file.
            String folderName = MeshFolderName;
            Debug.Log(String.Format("Saving mesh file: {0}", Path.Combine(folderName, fileName + fileExtension)));

            using (Stream stream = OpenFileForWrite(folderName, fileName + fileExtension)) {
                // Serialize and write the meshes to the file.
                byte[] data = SimpleMeshSerializer.Serialize(meshFilters);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            ExportRoomToWavefront();
            Debug.Log("Mesh file saved.");

            return Path.Combine(folderName, fileName + fileExtension);
        }

        /// <summary>
        /// Saves the provided meshes to the specified file.
        /// </summary>
        /// <param name="fileName">Name to give the saved mesh file. Exclude path and extension.</param>
        /// <param name="meshes">The collection of Mesh objects to save.</param>
        /// <returns>Fully qualified name of the saved mesh file.</returns>
        /// <remarks>Determines the save path to use and automatically applies the file extension.</remarks>
        public static string Save(string fileName, IEnumerable<Mesh> meshes) {
            if (string.IsNullOrEmpty(fileName)) {
                throw new ArgumentException("Must specify a valid fileName.");
            }

            if (meshes == null) {
                throw new ArgumentNullException("Value of meshes cannot be null.");
            }

            // Create the mesh file.
            String folderName = MeshFolderName;
            Debug.Log(String.Format("Saving mesh file: {0}", Path.Combine(folderName, fileName + fileExtension)));

            using (Stream stream = OpenFileForWrite(folderName, fileName + fileExtension)) {
                // Serialize and write the meshes to the file.
                byte[] data = SimpleMeshSerializer.Serialize(meshes);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }

            Debug.Log("Mesh file saved.");

            return Path.Combine(folderName, fileName + fileExtension);
        }

        /// <summary>
        /// Loads the specified mesh file.
        /// </summary>
        /// <param name="fileName">Name of the saved mesh file. Exclude path and extension.</param>
        /// <returns>Collection of Mesh objects read from the file.</returns>
        /// <remarks>Determines the path from which to load and automatically applies the file extension.</remarks>
        public static IList<Mesh> Load(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                throw new ArgumentException("Must specify a valid fileName.");
            }

            List<Mesh> meshes = new List<Mesh>();

            // Open the mesh file.
            String folderName = MeshFolderName;
            Debug.Log(String.Format("Loading mesh file: {0}", Path.Combine(folderName, fileName + fileExtension)));

            using (Stream stream = OpenFileForRead(folderName, fileName + fileExtension)) {
                // Read the file and deserialize the meshes.
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);

                meshes.AddRange(SimpleMeshSerializer.Deserialize(data));
            }

            Debug.Log("Mesh file loaded.");

            return meshes;
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="folderName">The name of the folder containing the file.</param>
        /// <param name="fileName">The name of the file, including extension. </param>
        /// <returns>Stream used for reading the file's data.</returns>
        private static Stream OpenFileForRead(string folderName, string fileName) {
            Stream stream = null;

#if !UNITY_EDITOR && UNITY_WSA
            Task<Task> task = Task<Task>.Factory.StartNew(
                            async () =>
                            {
                                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderName);
                                StorageFile file = await folder.GetFileAsync(fileName);
                                IRandomAccessStreamWithContentType randomAccessStream = await file.OpenReadAsync();
                                stream = randomAccessStream.AsStreamForRead();
                            });
            task.Wait();
            task.Result.Wait();
#else
            stream = new FileStream(Path.Combine(folderName, fileName), FileMode.Open, FileAccess.Read);
#endif
            return stream;
        }

        /// <summary>
        /// Opens the specified file for writing.
        /// </summary>
        /// <param name="folderName">The name of the folder containing the file.</param>
        /// <param name="fileName">The name of the file, including extension.</param>
        /// <returns>Stream used for writing the file's data.</returns>
        /// <remarks>If the specified file already exists, it will be overwritten.</remarks>
        private static Stream OpenFileForWrite(string folderName, string fileName) {
            Stream stream = null;

#if !UNITY_EDITOR && UNITY_WSA
            Task<Task> task = Task<Task>.Factory.StartNew(
                            async () =>
                            {
                                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderName);
                                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                IRandomAccessStream randomAccessStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                                stream = randomAccessStream.AsStreamForWrite();
                            });
            task.Wait();
            task.Result.Wait();
#else
            stream = new FileStream(Path.Combine(folderName, fileName), FileMode.Create, FileAccess.Write);
#endif
            return stream;
        }

        public static void ExportRoomToWavefront() {
            string fileName = Path.GetFileNameWithoutExtension("MeshModel");
            IEnumerable<Mesh> meshes = null;
            try {
                meshes = MeshSaver.Load(fileName);
            }
            catch {
                // Handling exceptions, and null returned by MeshSaver.Load, by checking if meshes
                // is still null below.
            }

            if (meshes == null) {
                return;
            }

            SaveMeshesToWavefront(fileName, meshes);

            // Open the location on where the mesh was saved.
            //System.Diagnostics.Process.Start(MeshFolderName);
        }


        /// <summary>
        /// Saves meshes without any modifications during serialization.
        /// </summary>
        /// <param name="fileName">Name of the file, without path and extension.</param>

        public static void SaveMeshesToWavefront(string fileName, IEnumerable<Mesh> meshes) {
            string filePath = Path.Combine(MeshFolderName, fileName + WavefrontFileExtension);
            int i = 1;
            while (File.Exists(filePath)) {
                i++;
                filePath = Path.Combine(MeshFolderName, fileName + " (" + i + ")" + WavefrontFileExtension);
            }
#if !UNITY_EDITOR && UNITY_WSA
                                var meshFolder =  ApplicationData.Current.RoamingFolder.Path;    
                                filePath = Path.Combine(meshFolder, fileName + WavefrontFileExtension);
                                i = 1;
                                while (File.Exists(filePath)) {
                                    i++;
                                    filePath = Path.Combine(MeshFolderName, fileName + " (" + i + ")" + WavefrontFileExtension);
                                }
#endif
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                var writer = new StreamWriter(stream, Encoding.UTF8);
                writer.Write(SerializeMeshes(meshes));
                writer.Flush();
            }
            /*
            using (StreamWriter stream = new StreamWriter(filePath)) {
                stream.Write(SerializeMeshes(meshes));
            }
            */

#if UNITY_EDITOR
            System.Diagnostics.Process.Start(MeshFolderName);
#endif
        }


        private static string SerializeMeshes(IEnumerable<Mesh> meshes) {
            StringWriter stream = new StringWriter();
            int offset = 0;
            foreach (var mesh in meshes) {
                SerializeMesh(mesh, stream, ref offset);
            }
            return stream.ToString();
        }


        /// <summary>
        /// Write single mesh to the stream passed in.
        /// </summary>
        /// <param name="meshFilter">Mesh to be serialized.</param>
        /// <param name="stream">Stream to write the mesh into.</param>
        /// <param name="offset">Index offset for handling multiple meshes in a single stream.</param>
        private static void SerializeMesh(Mesh mesh, TextWriter stream, ref int offset) {
            // Write vertices to .obj file. Need to make sure the points are transformed so everything is at a single origin.
            foreach (Vector3 vertex in mesh.vertices) {
                stream.WriteLine(string.Format("v {0} {1} {2}", -vertex.x, vertex.y, vertex.z));
            }

            // Write normals. Need to transform the direction.
            foreach (Vector3 normal in mesh.normals) {
                stream.WriteLine(string.Format("vn {0} {1} {2}", normal.x, normal.y, normal.z));
            }

            // Write indices.
            for (int s = 0, sLength = mesh.subMeshCount; s < sLength; ++s) {
                int[] indices = mesh.GetTriangles(s);
                for (int i = 0, iLength = indices.Length - indices.Length % 3; i < iLength; i += 3) {
                    // Format is "vertex index / material index / normal index"
                    stream.WriteLine(string.Format("f {0}//{0} {1}//{1} {2}//{2}",
                        indices[i + 2] + 1 + offset,
                        indices[i + 1] + 1 + offset,
                        indices[i + 0] + 1 + offset));
                }
            }

            offset += mesh.vertices.Length;
        }
    }
}
