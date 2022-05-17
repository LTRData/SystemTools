namespace ROOT.CIMV2; 
using System;
using System.ComponentModel;
using System.Management;
using System.Collections;
using System.Globalization;
using System.CodeDom.Compiler;


// Funktionerna ShouldSerialize<PropertyName> är funktioner som används av Egenskapsgranskning i VS för att kontrollera om en viss egenskap måste serialiseras. Dessa funktioner läggs till för alla ValueType-egenskaper (egenskaper av typen Int32, BOOL m.fl. som inte kan anges till Null). Dessa funktioner använder funktionen Is<PropertyName>Null. Funktionerna används också vid implementering av TypeConverter när NULL-värde kontrolleras för egenskapen, så att ett tomt värde kan visas i Egenskapsgranskning om Dra och släpp används i Visual Studio.
// Funktionerna Is<PropertyName>Null() används för att kontrollera om en egenskap är NULL.
// Funktionerna Reset<PropertyName> läggs till för Read/Write-egenskaper som kan ha värdet NULL. Dessa funktioner används i Egenskapsgranskning i VS-designer för att ange en egenskap till NULL.
// Varje egenskap som läggs till i klassen för WMI-egenskaper har angivna attribut som definierar dess beteende i Visual Studio-designer, och vilken TypeConverter som ska användas.
// En EarlyBound-klass genererades för WMI-klassen.SoftwareLicensingService
[GeneratedCode("mgmtclassgen", "")]
public class SoftwareLicensingService : System.ComponentModel.Component {
    
    // En privat egenskap som ska innehålla WMI-namnområdet där klassen finns.
    private static string CreatedWmiNamespace = "root\\CimV2";
    
    // En privat egenskap som ska innehålla namnet på den WMI-klass som skapade den här klassen.
    private static string CreatedClassName = "SoftwareLicensingService";
    
    // En privat medlemsvariabel som ska innehålla ManagementScope som används i olika metoder.
    private static System.Management.ManagementScope statMgmtScope = null;
    
    private ManagementSystemProperties PrivateSystemProperties;
    
    // Ett underliggande lateBound WMI-objekt.
    private System.Management.ManagementObject PrivateLateBoundObject;
    
    // En medlemsvariabel som lagrar klassens automatiskt aktiverade beteende.
    private bool AutoCommitProp;
    
    // En privat variabel som ska innehålla de inbäddade egenskaper som representerar instansen.
    private System.Management.ManagementBaseObject embeddedObj;
    
    // Det aktuella WMI-objekt som används
    private System.Management.ManagementBaseObject curObj;
    
    // En flagga som indikerar om instansen är ett inbäddat objekt.
    private bool isEmbedded;
    
    // Nedan visas olika överlagringar för konstruktörer som initierar en instans för klassen med ett WMI-objekt.
    public SoftwareLicensingService() {
        this.InitializeObject(null, null, null);
    }
    
    public SoftwareLicensingService(string keyVersion) {
        this.InitializeObject(null, new System.Management.ManagementPath(SoftwareLicensingService.ConstructPath(keyVersion)), null);
    }
    
    public SoftwareLicensingService(System.Management.ManagementScope mgmtScope, string keyVersion) {
        this.InitializeObject(((System.Management.ManagementScope)(mgmtScope)), new System.Management.ManagementPath(SoftwareLicensingService.ConstructPath(keyVersion)), null);
    }
    
    public SoftwareLicensingService(System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
        this.InitializeObject(null, path, getOptions);
    }
    
    public SoftwareLicensingService(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path) {
        this.InitializeObject(mgmtScope, path, null);
    }
    
    public SoftwareLicensingService(System.Management.ManagementPath path) {
        this.InitializeObject(null, path, null);
    }
    
    public SoftwareLicensingService(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
        this.InitializeObject(mgmtScope, path, getOptions);
    }
    
    public SoftwareLicensingService(System.Management.ManagementObject theObject) {
        Initialize();
        if ((CheckIfProperClass(theObject) == true)) {
            PrivateLateBoundObject = theObject;
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }
        else {
            throw new System.ArgumentException("Klassnamnet matchar inte.");
        }
    }
    
    public SoftwareLicensingService(System.Management.ManagementBaseObject theObject) {
        Initialize();
        if ((CheckIfProperClass(theObject) == true)) {
            embeddedObj = theObject;
            PrivateSystemProperties = new ManagementSystemProperties(theObject);
            curObj = embeddedObj;
            isEmbedded = true;
        }
        else {
            throw new System.ArgumentException("Klassnamnet matchar inte.");
        }
    }
    
    // En egenskap som returnerar WMI-klassens namnområde.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string OriginatingNamespace {
        get {
            return "root\\CimV2";
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ManagementClassName {
        get {
            string strRet = CreatedClassName;
            if ((curObj is not null)) {
                if ((curObj.ClassPath is not null)) {
                    strRet = ((string)(curObj["__CLASS"]));
                    if (((strRet is null) 
                                || (strRet == string.Empty))) {
                        strRet = CreatedClassName;
                    }
                }
            }
            return strRet;
        }
    }
    
    // Egenskaper som pekar till ett inbäddat objekt för att hämta WMI-objektets systemegenskaper.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ManagementSystemProperties SystemProperties {
        get {
            return PrivateSystemProperties;
        }
    }
    
    // En egenskap som returnerar det underliggande lateBound-objektet.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public System.Management.ManagementBaseObject LateBoundObject {
        get {
            return curObj;
        }
    }
    
    // Objektets ManagementScope.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public System.Management.ManagementScope Scope {
        get {
            if ((isEmbedded == false)) {
                return PrivateLateBoundObject.Scope;
            }
            else {
                return null;
            }
        }
        set {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Scope = value;
            }
        }
    }
    
    // Egenskap som visar aktiverat beteende för WMI-objektet. Om detta är True sparas WMI-objektet automatiskt efter varje egenskapsändring (d.v.s. Put() anropas efter en egenskapsändring).
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoCommit {
        get {
            return AutoCommitProp;
        }
        set {
            AutoCommitProp = value;
        }
    }
    
    // ManagementPath för det underliggande WMI-objektet.
    [Browsable(true)]
    public System.Management.ManagementPath Path {
        get {
            if ((isEmbedded == false)) {
                return PrivateLateBoundObject.Path;
            }
            else {
                return null;
            }
        }
        set {
            if ((isEmbedded == false)) {
                if ((CheckIfProperClass(null, value, null) != true)) {
                    throw new System.ArgumentException("Klassnamnet matchar inte.");
                }
                PrivateLateBoundObject.Path = value;
            }
        }
    }
    
    // En offentlig och statisk områdesegenskap som används i olika metoder.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static System.Management.ManagementScope StaticScope {
        get {
            return statMgmtScope;
        }
        set {
            statMgmtScope = value;
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The GUID that identifies a KMS client to a KMS host. The client includes this in " +
        "requests it sends to the KMS.")]
    public string ClientMachineID {
        get {
            return ((string)(curObj["ClientMachineID"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Last discovered KMS host IP address through DNS.")]
    public string DiscoveredKeyManagementServiceMachineIpAddress {
        get {
            return ((string)(curObj["DiscoveredKeyManagementServiceMachineIpAddress"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Last discovered KMS host name through DNS.")]
    public string DiscoveredKeyManagementServiceMachineName {
        get {
            return ((string)(curObj["DiscoveredKeyManagementServiceMachineName"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsDiscoveredKeyManagementServiceMachinePortNull {
        get {
            if ((curObj["DiscoveredKeyManagementServiceMachinePort"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Last discovered KMS host port through DNS.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint DiscoveredKeyManagementServiceMachinePort {
        get {
            if ((curObj["DiscoveredKeyManagementServiceMachinePort"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["DiscoveredKeyManagementServiceMachinePort"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsIsKeyManagementServiceMachineNull {
        get {
            if ((curObj["IsKeyManagementServiceMachine"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates whether KMS is enabled on the computer: 0 if false, 1 if true.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint IsKeyManagementServiceMachine {
        get {
            if ((curObj["IsKeyManagementServiceMachine"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["IsKeyManagementServiceMachine"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceCurrentCountNull {
        get {
            if ((curObj["KeyManagementServiceCurrentCount"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of currently active KMS clients on the KMS host. -1 indicates the compu" +
        "ter is not enabled as a KMS, or has not received any client licensing-requests.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceCurrentCount {
        get {
            if ((curObj["KeyManagementServiceCurrentCount"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceCurrentCount"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceDnsPublishingNull {
        get {
            if ((curObj["KeyManagementServiceDnsPublishing"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates the DNS publishing status of a KMS host: 0=Disabled, 1=Auto publish ena" +
        "bled (default).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool KeyManagementServiceDnsPublishing {
        get {
            if ((curObj["KeyManagementServiceDnsPublishing"] is null)) {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["KeyManagementServiceDnsPublishing"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceFailedRequestsNull {
        get {
            if ((curObj["KeyManagementServiceFailedRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of invalid KMS requests.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceFailedRequests {
        get {
            if ((curObj["KeyManagementServiceFailedRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceFailedRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceHostCachingNull {
        get {
            if ((curObj["KeyManagementServiceHostCaching"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates the caching status of KMS host name and port: 0=Caching disabled, 1=Cac" +
        "hing enabled (default).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool KeyManagementServiceHostCaching {
        get {
            if ((curObj["KeyManagementServiceHostCaching"] is null)) {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["KeyManagementServiceHostCaching"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceLicensedRequestsNull {
        get {
            if ((curObj["KeyManagementServiceLicensedRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of KMS requests from clients with License Status=1 (Licensed).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceLicensedRequests {
        get {
            if ((curObj["KeyManagementServiceLicensedRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceLicensedRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceListeningPortNull {
        get {
            if ((curObj["KeyManagementServiceListeningPort"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The TCP port the KMS host uses to listen for activation requests.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceListeningPort {
        get {
            if ((curObj["KeyManagementServiceListeningPort"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceListeningPort"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The FQDN of the resource domain containing the KMS SRV records for the org. Retur" +
        "ns null if SetKeyManagementServiceLookupDomain has not been called.")]
    public string KeyManagementServiceLookupDomain {
        get {
            return ((string)(curObj["KeyManagementServiceLookupDomain"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceLowPriorityNull {
        get {
            if ((curObj["KeyManagementServiceLowPriority"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates the thread priority status of KMS service: 0=Normal Priority (default)," +
        " 1=Low priority.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool KeyManagementServiceLowPriority {
        get {
            if ((curObj["KeyManagementServiceLowPriority"] is null)) {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["KeyManagementServiceLowPriority"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The name of the KMS host. Returns null if SetKeyManagementServiceMachine has not " +
        "been called.")]
    public string KeyManagementServiceMachine {
        get {
            return ((string)(curObj["KeyManagementServiceMachine"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceNonGenuineGraceRequestsNull {
        get {
            if ((curObj["KeyManagementServiceNonGenuineGraceRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of KMS requests from clients with License Status=4 (NonGenuineGrace).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceNonGenuineGraceRequests {
        get {
            if ((curObj["KeyManagementServiceNonGenuineGraceRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceNonGenuineGraceRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceNotificationRequestsNull {
        get {
            if ((curObj["KeyManagementServiceNotificationRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of KMS requests from clients with License Status=5 (Notification).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceNotificationRequests {
        get {
            if ((curObj["KeyManagementServiceNotificationRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceNotificationRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceOOBGraceRequestsNull {
        get {
            if ((curObj["KeyManagementServiceOOBGraceRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of KMS requests from clients with License Status=2 (OOBGrace).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceOOBGraceRequests {
        get {
            if ((curObj["KeyManagementServiceOOBGraceRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceOOBGraceRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceOOTGraceRequestsNull {
        get {
            if ((curObj["KeyManagementServiceOOTGraceRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of KMS requests from clients with License Status=3 (OOTGrace).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceOOTGraceRequests {
        get {
            if ((curObj["KeyManagementServiceOOTGraceRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceOOTGraceRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServicePortNull {
        get {
            if ((curObj["KeyManagementServicePort"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The TCP port used by clients to send KMS-activation requests. Returns 0 if SetKey" +
        "ManagementServicePort has not been called.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServicePort {
        get {
            if ((curObj["KeyManagementServicePort"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServicePort"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("KMS product key ID. Returns null if not applicable.")]
    public string KeyManagementServiceProductKeyID {
        get {
            return ((string)(curObj["KeyManagementServiceProductKeyID"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceTotalRequestsNull {
        get {
            if ((curObj["KeyManagementServiceTotalRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The total count of valid KMS requests.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceTotalRequests {
        get {
            if ((curObj["KeyManagementServiceTotalRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceTotalRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsKeyManagementServiceUnlicensedRequestsNull {
        get {
            if ((curObj["KeyManagementServiceUnlicensedRequests"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The count of KMS requests from clients with License Status=0 (Unlicensed).")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint KeyManagementServiceUnlicensedRequests {
        get {
            if ((curObj["KeyManagementServiceUnlicensedRequests"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["KeyManagementServiceUnlicensedRequests"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsOA2xBiosMarkerMinorVersionNull {
        get {
            if ((curObj["OA2xBiosMarkerMinorVersion"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Minor version number of the OA2 BIOS marker.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint OA2xBiosMarkerMinorVersion {
        get {
            if ((curObj["OA2xBiosMarkerMinorVersion"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["OA2xBiosMarkerMinorVersion"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsOA2xBiosMarkerStatusNull {
        get {
            if ((curObj["OA2xBiosMarkerStatus"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Status of the OA2 BIOS marker.  0=No SLIC table, 1=SLIC table with Windows marker" +
        ", 2=SLIC table without Windows marker, 3=Corrupted or invalid SLIC table")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint OA2xBiosMarkerStatus {
        get {
            if ((curObj["OA2xBiosMarkerStatus"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["OA2xBiosMarkerStatus"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Product key from the OA3 BIOS marker.")]
    public string OA3xOriginalProductKey {
        get {
            return ((string)(curObj["OA3xOriginalProductKey"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("SKU Version and Edition corresponding to the Key in the OA3 BIOS marker.")]
    public string OA3xOriginalProductKeyDescription {
        get {
            return ((string)(curObj["OA3xOriginalProductKeyDescription"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Part number of the Key in the OA3 BIOS marker.")]
    public string OA3xOriginalProductKeyPkPn {
        get {
            return ((string)(curObj["OA3xOriginalProductKeyPkPn"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsPolicyCacheRefreshRequiredNull {
        get {
            if ((curObj["PolicyCacheRefreshRequired"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates whether the licensing policy-cache needs to be updated: 0=not required," +
        " 1=Refresh required.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint PolicyCacheRefreshRequired {
        get {
            if ((curObj["PolicyCacheRefreshRequired"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["PolicyCacheRefreshRequired"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsRemainingWindowsReArmCountNull {
        get {
            if ((curObj["RemainingWindowsReArmCount"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Remaining number of times the client can be rearmed successfully.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint RemainingWindowsReArmCount {
        get {
            if ((curObj["RemainingWindowsReArmCount"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["RemainingWindowsReArmCount"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsRequiredClientCountNull {
        get {
            if ((curObj["RequiredClientCount"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The minimum number of clients required to connect to a KMS host in order to enabl" +
        "e volume licensing.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint RequiredClientCount {
        get {
            if ((curObj["RequiredClientCount"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["RequiredClientCount"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Additional information for token-based activation.")]
    public string TokenActivationAdditionalInfo {
        get {
            return ((string)(curObj["TokenActivationAdditionalInfo"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Thumbprint of the certificate that activated the machine.")]
    public string TokenActivationCertificateThumbprint {
        get {
            return ((string)(curObj["TokenActivationCertificateThumbprint"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsTokenActivationGrantNumberNull {
        get {
            if ((curObj["TokenActivationGrantNumber"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Grant number in the token-based activation license that activated the machine.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint TokenActivationGrantNumber {
        get {
            if ((curObj["TokenActivationGrantNumber"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["TokenActivationGrantNumber"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("ID of the token-based activation license that activated the machine.")]
    public string TokenActivationILID {
        get {
            return ((string)(curObj["TokenActivationILID"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsTokenActivationILVIDNull {
        get {
            if ((curObj["TokenActivationILVID"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Version of the token-based activation license that activated the machine.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint TokenActivationILVID {
        get {
            if ((curObj["TokenActivationILVID"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["TokenActivationILVID"]));
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Version of the Software Licensing service")]
    public string Version {
        get {
            return ((string)(curObj["Version"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsVLActivationIntervalNull {
        get {
            if ((curObj["VLActivationInterval"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The frequency, in minutes, of how often a client will contact the KMS host before" +
        " the client is licensed.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint VLActivationInterval {
        get {
            if ((curObj["VLActivationInterval"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["VLActivationInterval"]));
        }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsVLRenewalIntervalNull {
        get {
            if ((curObj["VLRenewalInterval"] is null)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The frequency, in minutes, of how often a client will contact the KMS host after " +
        "the client is licensed.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint VLRenewalInterval {
        get {
            if ((curObj["VLRenewalInterval"] is null)) {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["VLRenewalInterval"]));
        }
    }
    
    private bool CheckIfProperClass(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions OptionsParam) {
        if (((path is not null) 
                    && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
            return true;
        }
        else {
            return CheckIfProperClass(new System.Management.ManagementObject(mgmtScope, path, OptionsParam));
        }
    }
    
    private bool CheckIfProperClass(System.Management.ManagementBaseObject theObj) {
        if (((theObj is not null) 
                    && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
            return true;
        }
        else {
            System.Array parentClasses = ((System.Array)(theObj["__DERIVATION"]));
            if ((parentClasses is not null)) {
                int count = 0;
                for (count = 0; (count < parentClasses.Length); count = (count + 1)) {
                    if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    private bool ShouldSerializeDiscoveredKeyManagementServiceMachinePort() {
        if ((this.IsDiscoveredKeyManagementServiceMachinePortNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeIsKeyManagementServiceMachine() {
        if ((this.IsIsKeyManagementServiceMachineNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceCurrentCount() {
        if ((this.IsKeyManagementServiceCurrentCountNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceDnsPublishing() {
        if ((this.IsKeyManagementServiceDnsPublishingNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceFailedRequests() {
        if ((this.IsKeyManagementServiceFailedRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceHostCaching() {
        if ((this.IsKeyManagementServiceHostCachingNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceLicensedRequests() {
        if ((this.IsKeyManagementServiceLicensedRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceListeningPort() {
        if ((this.IsKeyManagementServiceListeningPortNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceLowPriority() {
        if ((this.IsKeyManagementServiceLowPriorityNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceNonGenuineGraceRequests() {
        if ((this.IsKeyManagementServiceNonGenuineGraceRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceNotificationRequests() {
        if ((this.IsKeyManagementServiceNotificationRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceOOBGraceRequests() {
        if ((this.IsKeyManagementServiceOOBGraceRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceOOTGraceRequests() {
        if ((this.IsKeyManagementServiceOOTGraceRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServicePort() {
        if ((this.IsKeyManagementServicePortNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceTotalRequests() {
        if ((this.IsKeyManagementServiceTotalRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeKeyManagementServiceUnlicensedRequests() {
        if ((this.IsKeyManagementServiceUnlicensedRequestsNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeOA2xBiosMarkerMinorVersion() {
        if ((this.IsOA2xBiosMarkerMinorVersionNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeOA2xBiosMarkerStatus() {
        if ((this.IsOA2xBiosMarkerStatusNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializePolicyCacheRefreshRequired() {
        if ((this.IsPolicyCacheRefreshRequiredNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeRemainingWindowsReArmCount() {
        if ((this.IsRemainingWindowsReArmCountNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeRequiredClientCount() {
        if ((this.IsRequiredClientCountNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeTokenActivationGrantNumber() {
        if ((this.IsTokenActivationGrantNumberNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeTokenActivationILVID() {
        if ((this.IsTokenActivationILVIDNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeVLActivationInterval() {
        if ((this.IsVLActivationIntervalNull == false)) {
            return true;
        }
        return false;
    }
    
    private bool ShouldSerializeVLRenewalInterval() {
        if ((this.IsVLRenewalIntervalNull == false)) {
            return true;
        }
        return false;
    }
    
    [Browsable(true)]
    public void CommitObject() {
        if ((isEmbedded == false)) {
            PrivateLateBoundObject.Put();
        }
    }
    
    [Browsable(true)]
    public void CommitObject(System.Management.PutOptions putOptions) {
        if ((isEmbedded == false)) {
            PrivateLateBoundObject.Put(putOptions);
        }
    }
    
    private void Initialize() {
        AutoCommitProp = true;
        isEmbedded = false;
    }
    
    private static string ConstructPath(string keyVersion) {
        string strPath = "root\\CimV2:SoftwareLicensingService";
        strPath = string.Concat(strPath, string.Concat(".Version=", string.Concat("\"", string.Concat(keyVersion, "\""))));
        return strPath;
    }
    
    private void InitializeObject(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
        Initialize();
        if ((path is not null)) {
            if ((CheckIfProperClass(mgmtScope, path, getOptions) != true)) {
                throw new System.ArgumentException("Klassnamnet matchar inte.");
            }
        }
        PrivateLateBoundObject = new System.Management.ManagementObject(mgmtScope, path, getOptions);
        PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
        curObj = PrivateLateBoundObject;
    }
    
    // Olika överlagringar av hjälp om GetInstances() i WMI-klassens uppräkningsinstanser.
    public static SoftwareLicensingServiceCollection GetInstances() {
        return GetInstances(null, null, null);
    }
    
    public static SoftwareLicensingServiceCollection GetInstances(string condition) {
        return GetInstances(null, condition, null);
    }
    
    public static SoftwareLicensingServiceCollection GetInstances(string[] selectedProperties) {
        return GetInstances(null, null, selectedProperties);
    }
    
    public static SoftwareLicensingServiceCollection GetInstances(string condition, string[] selectedProperties) {
        return GetInstances(null, condition, selectedProperties);
    }
    
    public static SoftwareLicensingServiceCollection GetInstances(System.Management.ManagementScope mgmtScope, System.Management.EnumerationOptions enumOptions) {
        if ((mgmtScope is null)) {
            if ((statMgmtScope is null)) {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = "root\\CimV2";
            }
            else {
                mgmtScope = statMgmtScope;
            }
        }
        System.Management.ManagementPath pathObj = new System.Management.ManagementPath();
        pathObj.ClassName = "SoftwareLicensingService";
        pathObj.NamespacePath = "root\\CimV2";
        System.Management.ManagementClass clsObject = new System.Management.ManagementClass(mgmtScope, pathObj, null);
        if ((enumOptions is null)) {
            enumOptions = new System.Management.EnumerationOptions();
            enumOptions.EnsureLocatable = true;
        }
        return new SoftwareLicensingServiceCollection(clsObject.GetInstances(enumOptions));
    }
    
    public static SoftwareLicensingServiceCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition) {
        return GetInstances(mgmtScope, condition, null);
    }
    
    public static SoftwareLicensingServiceCollection GetInstances(System.Management.ManagementScope mgmtScope, string[] selectedProperties) {
        return GetInstances(mgmtScope, null, selectedProperties);
    }
    
    public static SoftwareLicensingServiceCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition, string[] selectedProperties) {
        if ((mgmtScope is null)) {
            if ((statMgmtScope is null)) {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = "root\\CimV2";
            }
            else {
                mgmtScope = statMgmtScope;
            }
        }
        System.Management.ManagementObjectSearcher ObjectSearcher = new System.Management.ManagementObjectSearcher(mgmtScope, new SelectQuery("SoftwareLicensingService", condition, selectedProperties));
        System.Management.EnumerationOptions enumOptions = new System.Management.EnumerationOptions();
        enumOptions.EnsureLocatable = true;
        ObjectSearcher.Options = enumOptions;
        return new SoftwareLicensingServiceCollection(ObjectSearcher.Get());
    }
    
    [Browsable(true)]
    public static SoftwareLicensingService CreateInstance() {
        System.Management.ManagementScope mgmtScope = null;
        if ((statMgmtScope is null)) {
            mgmtScope = new System.Management.ManagementScope();
            mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
        }
        else {
            mgmtScope = statMgmtScope;
        }
        System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
        System.Management.ManagementClass tmpMgmtClass = new System.Management.ManagementClass(mgmtScope, mgmtPath, null);
        return new SoftwareLicensingService(tmpMgmtClass.CreateInstance());
    }
    
    [Browsable(true)]
    public void Delete() {
        PrivateLateBoundObject.Delete();
    }
    
    public uint AcquireGenuineTicket(string ServerUrl, string TemplateId) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("AcquireGenuineTicket");
            inParams["ServerUrl"] = ((string)(ServerUrl));
            inParams["TemplateId"] = ((string)(TemplateId));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("AcquireGenuineTicket", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ClearKeyManagementServiceListeningPort() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ClearKeyManagementServiceListeningPort", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ClearKeyManagementServiceLookupDomain() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ClearKeyManagementServiceLookupDomain", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ClearKeyManagementServiceMachine() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ClearKeyManagementServiceMachine", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ClearKeyManagementServicePort() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ClearKeyManagementServicePort", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ClearProductKeyFromRegistry() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ClearProductKeyFromRegistry", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ClearVLActivationTypeEnabled() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ClearVLActivationTypeEnabled", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint DepositActiveDirectoryOfflineActivationConfirmation(string ActivationObjectName, string ConfirmationID, string ProductKey) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("DepositActiveDirectoryOfflineActivationConfirmation");
            inParams["ActivationObjectName"] = ((string)(ActivationObjectName));
            inParams["ConfirmationID"] = ((string)(ConfirmationID));
            inParams["ProductKey"] = ((string)(ProductKey));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DepositActiveDirectoryOfflineActivationConfirmation", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint DisableKeyManagementServiceDnsPublishing(bool DisablePublishing) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("DisableKeyManagementServiceDnsPublishing");
            inParams["DisablePublishing"] = ((bool)(DisablePublishing));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DisableKeyManagementServiceDnsPublishing", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint DisableKeyManagementServiceHostCaching(bool DisableCaching) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("DisableKeyManagementServiceHostCaching");
            inParams["DisableCaching"] = ((bool)(DisableCaching));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DisableKeyManagementServiceHostCaching", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint DoActiveDirectoryOnlineActivation(string ActivationObjectName, string ProductKey) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("DoActiveDirectoryOnlineActivation");
            inParams["ActivationObjectName"] = ((string)(ActivationObjectName));
            inParams["ProductKey"] = ((string)(ProductKey));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DoActiveDirectoryOnlineActivation", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint EnableKeyManagementServiceLowPriority(bool EnableLowPriority) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("EnableKeyManagementServiceLowPriority");
            inParams["EnableLowPriority"] = ((bool)(EnableLowPriority));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("EnableKeyManagementServiceLowPriority", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint GenerateActiveDirectoryOfflineActivationId(string ProductKey, out string InstallationID) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("GenerateActiveDirectoryOfflineActivationId");
            inParams["ProductKey"] = ((string)(ProductKey));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GenerateActiveDirectoryOfflineActivationId", inParams, null);
            InstallationID = System.Convert.ToString(outParams.Properties["InstallationID"].Value);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            InstallationID = null;
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint InstallLicense(string License) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("InstallLicense");
            inParams["License"] = ((string)(License));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("InstallLicense", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint InstallLicensePackage(string LicensePackage) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("InstallLicensePackage");
            inParams["LicensePackage"] = ((string)(LicensePackage));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("InstallLicensePackage", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint InstallProductKey(string ProductKey) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("InstallProductKey");
            inParams["ProductKey"] = ((string)(ProductKey));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("InstallProductKey", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ReArmApp(string ApplicationId) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("ReArmApp");
            inParams["ApplicationId"] = ((string)(ApplicationId));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ReArmApp", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint ReArmWindows() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ReArmWindows", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint RefreshLicenseStatus() {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("RefreshLicenseStatus", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint SetKeyManagementServiceListeningPort(uint PortNumber) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetKeyManagementServiceListeningPort");
            inParams["PortNumber"] = ((uint)(PortNumber));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetKeyManagementServiceListeningPort", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint SetKeyManagementServiceLookupDomain(string LookupDomain) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetKeyManagementServiceLookupDomain");
            inParams["LookupDomain"] = ((string)(LookupDomain));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetKeyManagementServiceLookupDomain", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint SetKeyManagementServiceMachine(string MachineName) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetKeyManagementServiceMachine");
            inParams["MachineName"] = ((string)(MachineName));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetKeyManagementServiceMachine", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint SetKeyManagementServicePort(uint PortNumber) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetKeyManagementServicePort");
            inParams["PortNumber"] = ((uint)(PortNumber));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetKeyManagementServicePort", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint SetVLActivationInterval(uint ActivationInterval) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetVLActivationInterval");
            inParams["ActivationInterval"] = ((uint)(ActivationInterval));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetVLActivationInterval", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint SetVLActivationTypeEnabled(uint ActivationType) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetVLActivationTypeEnabled");
            inParams["ActivationType"] = ((uint)(ActivationType));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetVLActivationTypeEnabled", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    public uint SetVLRenewalInterval(uint RenewalInterval) {
        if ((isEmbedded == false)) {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetVLRenewalInterval");
            inParams["RenewalInterval"] = ((uint)(RenewalInterval));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetVLRenewalInterval", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else {
            return System.Convert.ToUInt32(0);
        }
    }
    
    // Implementering av uppräknare för klassens uppräkningsinstanser.
    public class SoftwareLicensingServiceCollection : object, ICollection {
        
        private ManagementObjectCollection privColObj;
        
        public SoftwareLicensingServiceCollection(ManagementObjectCollection objCollection) {
            privColObj = objCollection;
        }
        
        public virtual int Count {
            get {
                return privColObj.Count;
            }
        }
        
        public virtual bool IsSynchronized {
            get {
                return privColObj.IsSynchronized;
            }
        }
        
        public virtual object SyncRoot {
            get {
                return this;
            }
        }
        
        public virtual void CopyTo(System.Array array, int index) {
            privColObj.CopyTo(array, index);
            int nCtr;
            for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1)) {
                array.SetValue(new SoftwareLicensingService(((System.Management.ManagementObject)(array.GetValue(nCtr)))), nCtr);
            }
        }
        
        public virtual System.Collections.IEnumerator GetEnumerator() {
            return new SoftwareLicensingServiceEnumerator(privColObj.GetEnumerator());
        }
        
        public class SoftwareLicensingServiceEnumerator : object, System.Collections.IEnumerator {
            
            private ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;
            
            public SoftwareLicensingServiceEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum) {
                privObjEnum = objEnum;
            }
            
            public virtual object Current {
                get {
                    return new SoftwareLicensingService(((System.Management.ManagementObject)(privObjEnum.Current)));
                }
            }
            
            public virtual bool MoveNext() {
                return privObjEnum.MoveNext();
            }
            
            public virtual void Reset() {
                privObjEnum.Reset();
            }
        }
    }
    
    // TypeConverter som hanterar Null-värden för ValueType-egenskaper
    public class WMIValueTypeConverter : TypeConverter {
        
        private TypeConverter baseConverter;
        
        private System.Type baseType;
        
        public WMIValueTypeConverter(System.Type inBaseType) {
            baseConverter = TypeDescriptor.GetConverter(inBaseType);
            baseType = inBaseType;
        }
        
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type srcType) {
            return baseConverter.CanConvertFrom(context, srcType);
        }
        
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
            return baseConverter.CanConvertTo(context, destinationType);
        }
        
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            return baseConverter.ConvertFrom(context, culture, value);
        }
        
        public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary dictionary) {
            return baseConverter.CreateInstance(context, dictionary);
        }
        
        public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context) {
            return baseConverter.GetCreateInstanceSupported(context);
        }
        
        public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributeVar) {
            return baseConverter.GetProperties(context, value, attributeVar);
        }
        
        public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context) {
            return baseConverter.GetPropertiesSupported(context);
        }
        
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context) {
            return baseConverter.GetStandardValues(context);
        }
        
        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context) {
            return baseConverter.GetStandardValuesExclusive(context);
        }
        
        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context) {
            return baseConverter.GetStandardValuesSupported(context);
        }
        
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
            if ((baseType.BaseType == typeof(System.Enum))) {
                if ((value.GetType() == destinationType)) {
                    return value;
                }
                if ((((value is null) 
                            && (context is not null)) 
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                    return  "NULL_ENUM_VALUE" ;
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if (((baseType == typeof(bool)) 
                        && (baseType.BaseType == typeof(System.ValueType)))) {
                if ((((value is null) 
                            && (context is not null)) 
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if (((context is not null) 
                        && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                return "";
            }
            return baseConverter.ConvertTo(context, culture, value, destinationType);
        }
    }
    
    // En inbäddad klass som representerar WMI-systemegenskaper.
    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public class ManagementSystemProperties {
        
        private System.Management.ManagementBaseObject PrivateLateBoundObject;
        
        public ManagementSystemProperties(System.Management.ManagementBaseObject ManagedObject) {
            PrivateLateBoundObject = ManagedObject;
        }
        
        [Browsable(true)]
        public int GENUS {
            get {
                return ((int)(PrivateLateBoundObject["__GENUS"]));
            }
        }
        
        [Browsable(true)]
        public string CLASS {
            get {
                return ((string)(PrivateLateBoundObject["__CLASS"]));
            }
        }
        
        [Browsable(true)]
        public string SUPERCLASS {
            get {
                return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
            }
        }
        
        [Browsable(true)]
        public string DYNASTY {
            get {
                return ((string)(PrivateLateBoundObject["__DYNASTY"]));
            }
        }
        
        [Browsable(true)]
        public string RELPATH {
            get {
                return ((string)(PrivateLateBoundObject["__RELPATH"]));
            }
        }
        
        [Browsable(true)]
        public int PROPERTY_COUNT {
            get {
                return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
            }
        }
        
        [Browsable(true)]
        public string[] DERIVATION {
            get {
                return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
            }
        }
        
        [Browsable(true)]
        public string SERVER {
            get {
                return ((string)(PrivateLateBoundObject["__SERVER"]));
            }
        }
        
        [Browsable(true)]
        public string NAMESPACE {
            get {
                return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
            }
        }
        
        [Browsable(true)]
        public string PATH {
            get {
                return ((string)(PrivateLateBoundObject["__PATH"]));
            }
        }
    }
}
