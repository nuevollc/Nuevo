using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.CDR.LD
{
    public class CallInfo
    {
        // call parameters
        private DateTime _startTime;
        private DateTime _answerTime;
        private DateTime _releaseTime;
        private string _callingNumber;
        private string _calledNumber;
        private int _callDuration;
        private string _group;
        private int _id;
        private bool _isLocal = false;

        // accessors
        public DateTime StartTime
        {
            get
            {
                return this._startTime;
            }
            set
            {
                _startTime = value;

            }

        }//_startTime
        public DateTime AnswerTime
        {
            get
            {
                return this._answerTime;
            }
            set
            {
                _answerTime = value;

            }

        }//_answerTime
        public DateTime ReleaseTime
        {
            get
            {
                return this._releaseTime;
            }
            set
            {
                _releaseTime = value;

            }

        }//_releaseTime
        public string CallingNumber
        {
            get
            {
                return this._callingNumber;
            }
            set
            {
                _callingNumber = value;

            }

        }//_callingNumber
        public string CalledNumber
        {
            get
            {
                return this._calledNumber;
            }
            set
            {
                _calledNumber = value;

            }

        }//_calledNumber
        public int CallDuration
        {
            get
            {
                return this._callDuration;
            }
            set
            {
                _callDuration = value;

            }

        }//_callDuration              
        public bool LocalCall
        {
            get
            {
                return this._isLocal;
            }
            set
            {
                _isLocal = value;

            }

        }//_isLocal
        public string Group
        {
            get
            {
                return this._group;
            }
            set
            {
                _group = value;

            }

        }//_group
        public int Id
        {
            get
            {
                return this._id;
            }
            set
            {
                _id = value;

            }

        }//_callDuration              
  

    }


}
