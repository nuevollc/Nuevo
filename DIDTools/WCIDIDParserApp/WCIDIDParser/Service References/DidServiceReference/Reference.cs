﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCIDIDParser.DidServiceReference {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DidServiceReference.DidServiceSoap")]
    public interface DidServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetServiceProviders", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetServiceProviders();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetGroups", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetGroups();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetPhoneNumberStates", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetPhoneNumberStates();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetCarriers", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetCarriers();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetPhoneNumberTypes", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetPhoneNumberTypes();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetAccessDeviceList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetAccessDeviceList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetServicePackList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetServicePackList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDIDsForServiceProvider", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetDIDsForServiceProvider(string theSP);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDIDsOfType", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetDIDsOfType(int typeID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDIDsForGroup", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetDIDsForGroup(string groupId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetAvailableDIDsByCarrier", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetAvailableDIDsByCarrier(string carrierId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDID", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetDID(string theDID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetAvailableDIDsByNPA", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetAvailableDIDsByNPA(string theNPA);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetAvailableDIDsByNPANXX", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        System.Data.DataSet GetAvailableDIDsByNPANXX(string thenpa, string thenxx);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UpdateDIDState", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool UpdateDIDState(string did, int stateId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AssignPhoneNumberToServiceProvider", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AssignPhoneNumberToServiceProvider(string phoneNumber, string spId, string stateId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AssignPhoneNumberToServiceProviderGroup", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AssignPhoneNumberToServiceProviderGroup(string phoneNumber, string spId, string groupId, string stateId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AssignPhoneNumberRangeToServiceProviderGroup", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AssignPhoneNumberRangeToServiceProviderGroup(string phoneNumber, string endingPhoneNumber, string spId, string groupId, string stateId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AssignPhoneNumberRangeToServiceProvider", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AssignPhoneNumberRangeToServiceProvider(string phoneNumber, string endingPhoneNumber, string spId, string stateId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DeletePhoneNumber", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool DeletePhoneNumber(string phoneNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AddPhoneNumber", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AddPhoneNumber(string phoneNumber, string carrierId, string typeId, string stateId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AddPhoneNumberToServiceProvider", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AddPhoneNumberToServiceProvider(string phoneNumber, string carrierId, string typeId, string stateId, string spId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AddPhoneNumberToServiceProviderGroup", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AddPhoneNumberToServiceProviderGroup(string phoneNumber, string carrierId, string typeId, string stateId, string spId, string groupId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AddPhoneNumberRange", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AddPhoneNumberRange(string phoneNumber, string endingPhoneNumber, string carrierId, string typeId, string stateId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AddPhoneNumberRangeToServiceProvider", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AddPhoneNumberRangeToServiceProvider(string phoneNumber, string endingPhoneNumber, string carrierId, string typeId, string stateId, string spId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AddPhoneNumberRangeToServiceProviderGroup", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool AddPhoneNumberRangeToServiceProviderGroup(string phoneNumber, string endingPhoneNumber, string carrierId, string typeId, string stateId, string spId, string groupId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DeletePhoneNumberRange", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool DeletePhoneNumberRange(string phoneNumber, string endingPhoneNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoadDidsViaFile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool LoadDidsViaFile(string fileName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface DidServiceSoapChannel : WCIDIDParser.DidServiceReference.DidServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class DidServiceSoapClient : System.ServiceModel.ClientBase<WCIDIDParser.DidServiceReference.DidServiceSoap>, WCIDIDParser.DidServiceReference.DidServiceSoap {
        
        public DidServiceSoapClient() {
        }
        
        public DidServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DidServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DidServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DidServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataSet GetServiceProviders() {
            return base.Channel.GetServiceProviders();
        }
        
        public System.Data.DataSet GetGroups() {
            return base.Channel.GetGroups();
        }
        
        public System.Data.DataSet GetPhoneNumberStates() {
            return base.Channel.GetPhoneNumberStates();
        }
        
        public System.Data.DataSet GetCarriers() {
            return base.Channel.GetCarriers();
        }
        
        public System.Data.DataSet GetPhoneNumberTypes() {
            return base.Channel.GetPhoneNumberTypes();
        }
        
        public System.Data.DataSet GetAccessDeviceList() {
            return base.Channel.GetAccessDeviceList();
        }
        
        public System.Data.DataSet GetServicePackList() {
            return base.Channel.GetServicePackList();
        }
        
        public System.Data.DataSet GetDIDsForServiceProvider(string theSP) {
            return base.Channel.GetDIDsForServiceProvider(theSP);
        }
        
        public System.Data.DataSet GetDIDsOfType(int typeID) {
            return base.Channel.GetDIDsOfType(typeID);
        }
        
        public System.Data.DataSet GetDIDsForGroup(string groupId) {
            return base.Channel.GetDIDsForGroup(groupId);
        }
        
        public System.Data.DataSet GetAvailableDIDsByCarrier(string carrierId) {
            return base.Channel.GetAvailableDIDsByCarrier(carrierId);
        }
        
        public System.Data.DataSet GetDID(string theDID) {
            return base.Channel.GetDID(theDID);
        }
        
        public System.Data.DataSet GetAvailableDIDsByNPA(string theNPA) {
            return base.Channel.GetAvailableDIDsByNPA(theNPA);
        }
        
        public System.Data.DataSet GetAvailableDIDsByNPANXX(string thenpa, string thenxx) {
            return base.Channel.GetAvailableDIDsByNPANXX(thenpa, thenxx);
        }
        
        public bool UpdateDIDState(string did, int stateId) {
            return base.Channel.UpdateDIDState(did, stateId);
        }
        
        public bool AssignPhoneNumberToServiceProvider(string phoneNumber, string spId, string stateId) {
            return base.Channel.AssignPhoneNumberToServiceProvider(phoneNumber, spId, stateId);
        }
        
        public bool AssignPhoneNumberToServiceProviderGroup(string phoneNumber, string spId, string groupId, string stateId) {
            return base.Channel.AssignPhoneNumberToServiceProviderGroup(phoneNumber, spId, groupId, stateId);
        }
        
        public bool AssignPhoneNumberRangeToServiceProviderGroup(string phoneNumber, string endingPhoneNumber, string spId, string groupId, string stateId) {
            return base.Channel.AssignPhoneNumberRangeToServiceProviderGroup(phoneNumber, endingPhoneNumber, spId, groupId, stateId);
        }
        
        public bool AssignPhoneNumberRangeToServiceProvider(string phoneNumber, string endingPhoneNumber, string spId, string stateId) {
            return base.Channel.AssignPhoneNumberRangeToServiceProvider(phoneNumber, endingPhoneNumber, spId, stateId);
        }
        
        public bool DeletePhoneNumber(string phoneNumber) {
            return base.Channel.DeletePhoneNumber(phoneNumber);
        }
        
        public bool AddPhoneNumber(string phoneNumber, string carrierId, string typeId, string stateId) {
            return base.Channel.AddPhoneNumber(phoneNumber, carrierId, typeId, stateId);
        }
        
        public bool AddPhoneNumberToServiceProvider(string phoneNumber, string carrierId, string typeId, string stateId, string spId) {
            return base.Channel.AddPhoneNumberToServiceProvider(phoneNumber, carrierId, typeId, stateId, spId);
        }
        
        public bool AddPhoneNumberToServiceProviderGroup(string phoneNumber, string carrierId, string typeId, string stateId, string spId, string groupId) {
            return base.Channel.AddPhoneNumberToServiceProviderGroup(phoneNumber, carrierId, typeId, stateId, spId, groupId);
        }
        
        public bool AddPhoneNumberRange(string phoneNumber, string endingPhoneNumber, string carrierId, string typeId, string stateId) {
            return base.Channel.AddPhoneNumberRange(phoneNumber, endingPhoneNumber, carrierId, typeId, stateId);
        }
        
        public bool AddPhoneNumberRangeToServiceProvider(string phoneNumber, string endingPhoneNumber, string carrierId, string typeId, string stateId, string spId) {
            return base.Channel.AddPhoneNumberRangeToServiceProvider(phoneNumber, endingPhoneNumber, carrierId, typeId, stateId, spId);
        }
        
        public bool AddPhoneNumberRangeToServiceProviderGroup(string phoneNumber, string endingPhoneNumber, string carrierId, string typeId, string stateId, string spId, string groupId) {
            return base.Channel.AddPhoneNumberRangeToServiceProviderGroup(phoneNumber, endingPhoneNumber, carrierId, typeId, stateId, spId, groupId);
        }
        
        public bool DeletePhoneNumberRange(string phoneNumber, string endingPhoneNumber) {
            return base.Channel.DeletePhoneNumberRange(phoneNumber, endingPhoneNumber);
        }
        
        public bool LoadDidsViaFile(string fileName) {
            return base.Channel.LoadDidsViaFile(fileName);
        }
    }
}
