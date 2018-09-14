#-------------------------------------------------------------------- 
# Name: Load CSV into SharePoint List 
#--------------------------------------------------------------------



# Setup the correct modules for SharePoint Manipulation 
if ( (Get-PSSnapin -Name Microsoft.SharePoint.PowerShell -ErrorAction SilentlyContinue) -eq $null ) 
{ 
   Add-PsSnapin Microsoft.SharePoint.PowerShell 
} 
$host.Runspace.ThreadOptions = "ReuseThread" 

#Open SharePoint List 
$SPServer=http://kndev-stage
$SPAppList="/Lists/News%20Articles/" 
$spWeb = Get-SPWeb $SPServer 
$spData = $spWeb.GetList($SPAppList) 

$InvFile="NameOfExcell.csv" 
# Get Data from Excell CSV File 
$FileExists = (Test-Path $InvFile -PathType Leaf) 
if ($FileExists) { 
   "Loading $InvFile for processing..." 
   $tblData = Import-CSV $InvFile 
} else { 
   "$InvFile not found - stopping import!" 
   exit 
} 

# Loop through Applications add each one to SharePoint 

"Uploading data to SharePoint...." 

foreach ($row in $tblData) 
{ 
   "Adding entry for "+$row."Application Name".ToString() 
   $spItem = $spData.AddItem() 
   $spItem["Title"] = $row."Title".ToString() 
   $spItem["News Articles Titles"] = $row."NEWS_ITM_HDLN_TXT".ToString()
   $spItem["News Article"] = $row."Body".ToString() 
  # $spItem["Article Publishing Date"] = $row."Matching_one_in_CSV".ToString() 
  # $spItem["Publishing Checker"] = $row."Matching_one_in_CSV"
  # $spItem["Mail Sent"] = $row."Matching_one_in_CSV"
   $spItem.Update() 
} 

"---------------" 
"Upload Complete" 

$spWeb.Dispose()
