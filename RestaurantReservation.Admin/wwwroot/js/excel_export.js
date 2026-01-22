// annualReport.js
// Provides a function for downloading a file from a base64 string (Excel export)
window.downloadFileFromBytes = (filename, base64) =>
{
    const link = document.createElement('a');
    link.download = filename;
    link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,' + base64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}; 