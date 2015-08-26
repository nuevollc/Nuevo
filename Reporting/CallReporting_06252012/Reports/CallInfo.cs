using System;

namespace TruMobility.Reporting.CDR
{
    public class CallInfo
    {
        private string _userNumber;
        private string _calledNumber;
        private string _callingNumber;
        private DateTime _callDate;
        private string _direction;
        private TimeSpan _duration;
        private string _answerIndicator;

        public string UserNumber
        {
            get
            {
                return this._userNumber;
            }
            set
            {
                _userNumber = value;

            }

        }//_userNumber

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

        public DateTime CallDate
        {

            get
            {
                return this._callDate;
            }
            set
            {
                _callDate = value;

            }

        }//_callDate

        public string Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                _direction = value;

            }

        } //_direction

        public TimeSpan Duration
        {

            get
            {
                return this._duration;
            }
            set
            {
                _duration = value;

            }

        }//_duration

        public string AnswerIndicator
        {

            get
            {
                return this._answerIndicator;
            }
            set
            {
                _answerIndicator = value;

            }

        }//_answerIndicator

    }
}
