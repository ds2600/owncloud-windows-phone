﻿using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using OwnCloud.Resource;
using OwnCloud.Extensions;

namespace OwnCloud.Data
{
    public class File : Entity
    {
        private string _name = "";
        /// <summary>
        /// Current Name of file
        /// </summary>
        public string FileName
        {
            get
            {
                return Uri.UnescapeDataString(_name);
            }
            set
            {
                if (value == String.Empty)
                {
                    throw new Exception("Model_File_Filename_Empty".Translate());
                }
                else
                {
                    _name = value;
                }
            }
        }

        /// <summary>
        /// The relative resource location
        /// </summary>
        private string _path = "";
        public string FilePath
        {
            get { return _path; }
            set
            {
                _path = value;
            }
        }

        private long _size = 0;
        /// <summary>
        /// Size of file
        /// </summary>
        public long FileSize
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Gets the filesize in readable format by adding KB, MB etc
        /// </summary>
        public string Size
        {
            get
            {
                return Utility.FormatBytes(_size);
            }
        }

        private string _type = "";
        /// <summary>
        /// Mime type of file
        /// </summary>
        public string FileType
        {
            get { return _type; }
            set { _type = value.ToString(); }
        }

        /// <summary>
        /// Gets the file type
        /// </summary>
        public string Type
        {
            get
            {
                if (IsDirectory)
                {
                    return "Directory".Translate();
                }
                else
                {
                    return MimeTypes.GetNameOf(_type.Split(';')[0]);
                }
            }
        }

        bool _synced = false;
        /// <summary>
        /// Tells if the file is in sync state
        /// </summary>
        public bool IsSynced
        {
            get
            {
                return _synced;
            }
        }

        /// <summary>
        /// Tells or sets if the file should be synced
        /// </summary>
        public bool Sync
        {
            get;
            set;
        }

        /// <summary>
        /// Tells or sets if the file should be treated as directory
        /// </summary>
        public bool IsDirectory
        {
            get;
            set;
        }

        DateTime _mtime;
        /// <summary>
        /// Gets or sets the last modification time
        /// </summary>
        public DateTime FileLastModified
        {
            get
            {
                return _mtime == null ? new DateTime() : _mtime;
            }
            set
            {
                _mtime = value;
            }
        }

        DateTime _ctime;
        /// <summary>
        /// Gets or sets the creation time
        /// </summary>
        public DateTime FileCreated
        {
            get
            {
                return _ctime == null ? new DateTime() : _ctime;
            }
            set
            {
                _ctime = value;
            }
        }

        /// <summary>
        /// Gets the last modification time of the file in readable format
        /// </summary>
        public string LastModified
        {
            get
            {
                return "✎ " + FileLastModified.ToString("");
            }
        }

        /// <summary>
        /// Gets the creation time of the file in readable format
        /// </summary>
        public string Created
        {
            get
            {
                return "✎ " + FileCreated.ToString("");
            }
        }

        static Dictionary<string, Uri> _iconCache;


        /// <summary>
        /// Tries to load a icon from Assets/FileIcon where the MIME-type of the file
        /// is correspondencing to icon value.
        /// The MIME-type must be encoded to filename where "/" will be encoded to "_"
        /// For exmaple "text/plain" will be encoded to "text_plain", "application/vnd.something" results in "application_vnd.something"
        /// A directory is referenced by "folder.png".
        /// A unknown MIME is responding in "file.png" where a MIME-fallback is improved. For example "audio/mpeg" will fallback to "audio.png" main-type
        /// on non existing files before falling back fo "file.png".
        /// </summary>
        public Uri ImageIcon
        {
            get
            {
                if (_iconCache == null) _iconCache = new Dictionary<string, Uri>();
                Uri image = null;
                if (IsDirectory)
                {
                    return new Uri("/Assets/FileIcons/folder.png", UriKind.Relative);
                }
                else
                {
                    string[] filenames = new string[2];

                    if (!String.IsNullOrWhiteSpace(FileType))
                    {
                        if (_iconCache.ContainsKey(FileType))
                        {
                            return _iconCache[FileType];
                        }

                        filenames[0] = "/Assets/FileIcons/" + FileType.Replace("/", "_") + ".png";
                        filenames[1] = "/Assets/FileIcons/" + FileType.Split('/')[0] + ".png";

                        foreach (string filename in filenames)
                        {
                            if (ResourceLoader.ResourceExists(filename))
                            {
                                // need a slash here if not given
                                var uri = ResourceLoader.GetStreamUri(filename).ToString().Trim('/');
                                image = new Uri("/" + uri , UriKind.Relative);
                                break;
                            }
                        }

                        if (image == null)
                        {
                           image = new Uri("/Assets/FileIcons/file.png", UriKind.Relative);
                        }

                        if (!_iconCache.ContainsKey(FileType))
                        {
                            _iconCache.Add(FileType, image);
                        }

                        return image;
                    }
                    else
                    {
                        return ResourceLoader.GetStreamUri("/Assets/FileIcons/file.png");
                    }
                }
            }
        }
    }
}