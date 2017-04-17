﻿#region usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Convex.ComponentModel;

#endregion

namespace Convex.Types {
    public class User : MarshalByRefObject, INotifyPropertyChanged {
        private int access;
        private int attempts;
        private int id;
        private string nickname;
        private string realname;
        private DateTime seen;

        public User(int id, string nickname, string realname, int access, DateTime seen) {
            Access = access;
            Nickname = nickname;
            Realname = realname;
            Seen = seen;
            Id = id;

            Messages = new List<Message>();
            Channels = new List<string>();
        }

        public int Id {
            get { return id; }
            set {
                id = value;
                NotifyPropertyChanged(value);
            }
        }

        public int Attempts {
            get { return attempts; }
            set {
                attempts = value;
                NotifyPropertyChanged(value);
            }
        }

        public string Nickname {
            get { return nickname; }
            set {
                nickname = value;
                NotifyPropertyChanged(value);
            }
        }

        public string Realname {
            get { return realname; }
            set {
                realname = value;
                NotifyPropertyChanged(value);
            }
        }

        public int Access {
            get { return access; }
            set {
                access = value;
                NotifyPropertyChanged(value);
            }
        }

        public DateTime Seen {
            get { return seen; }
            set {
                seen = value;
                NotifyPropertyChanged(value);
            }
        }

        public List<Message> Messages { get; }
        public List<string> Channels { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Discern whether a user has exceeded command-querying limit
        /// </summary>
        /// <returns>true: user timeout</returns>
        public bool GetTimeout() {
            bool doTimeout = false;

            if (Attempts.Equals(4))
                if (Seen.AddMinutes(1) < DateTime.UtcNow)
                    Attempts = 0; // if so, reset their attempts to 0
                else
                    doTimeout = true; // if not, timeout is true
            else if (Access > 1)
                // if user isn't admin/op, increment their attempts
                Attempts++;

            return doTimeout;
        }

        protected void NotifyPropertyChanged(object newValue, [CallerMemberName] string memberName = "") {
            OnPropertyChanged(this, new SpecialPropertyChangedEventArgs(memberName, Realname, newValue));
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            PropertyChanged?.Invoke(this, e);
        }
    }

    public class Message {
        public Message(string sender, string contents, DateTime timestamp) {
            Sender = sender;
            Contents = contents;
            Date = timestamp;
        }

        public string Sender { get; set; }
        public string Contents { get; set; }
        public DateTime Date { get; set; }
    }
}