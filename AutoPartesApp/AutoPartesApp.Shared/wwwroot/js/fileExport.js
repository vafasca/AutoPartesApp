// ========================================
// FILE EXPORT UTILITIES
// ========================================

/**
 * Descarga un archivo desde base64
 * @param {string} filename - Nombre del archivo
 * @param {string} contentType - MIME type (ej: 'text/csv', 'application/pdf')
 * @param {string} base64Content - Contenido en base64
 */
window.downloadFile = function (filename, contentType, base64Content) {
    try {
        // Decodificar base64
        const byteCharacters = atob(base64Content);
        const byteNumbers = new Array(byteCharacters.length);
        
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: contentType });
        
        // Crear link de descarga
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        
        document.body.appendChild(a);
        a.click();
        
        // Limpiar
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        
        console.log(`✅ Archivo descargado: ${filename}`);
    } catch (error) {
        console.error('❌ Error al descargar archivo:', error);
        throw error;
    }
};

/**
 * Exporta datos a PDF usando jsPDF
 * @param {string} title - Título del reporte
 * @param {string} jsonData - Datos en formato JSON string
 * @param {string} filename - Nombre del archivo
 */
window.exportToPdf = function (title, jsonData, filename) {
    try {
        // Verificar si jsPDF está disponible
        if (typeof jsPDF === 'undefined') {
            console.warn('⚠️ jsPDF no está cargado. Usando exportación básica.');
            exportBasicPdf(title, jsonData, filename);
            return;
        }

        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        const data = JSON.parse(jsonData);

        // Configuración
        let yPosition = 20;
        const pageWidth = doc.internal.pageSize.getWidth();
        const margin = 15;

        // Título
        doc.setFontSize(18);
        doc.setFont(undefined, 'bold');
        doc.text(title, pageWidth / 2, yPosition, { align: 'center' });
        yPosition += 15;

        // Fecha
        doc.setFontSize(10);
        doc.setFont(undefined, 'normal');
        doc.text(`Generado: ${new Date().toLocaleString('es-ES')}`, pageWidth / 2, yPosition, { align: 'center' });
        yPosition += 15;

        // Línea separadora
        doc.setLineWidth(0.5);
        doc.line(margin, yPosition, pageWidth - margin, yPosition);
        yPosition += 10;

        // Contenido del reporte
        doc.setFontSize(12);
        doc.setFont(undefined, 'normal');

        const content = JSON.stringify(data, null, 2);
        const lines = doc.splitTextToSize(content, pageWidth - (margin * 2));

        lines.forEach(line => {
            if (yPosition > 280) {
                doc.addPage();
                yPosition = 20;
            }
            doc.text(line, margin, yPosition);
            yPosition += 7;
        });

        // Descargar
        doc.save(filename);
        console.log(`✅ PDF generado: ${filename}`);

    } catch (error) {
        console.error('❌ Error al exportar PDF:', error);
        // Fallback: exportación básica
        exportBasicPdf(title, jsonData, filename);
    }
};

/**
 * Exportación básica de PDF (sin jsPDF)
 */
function exportBasicPdf(title, jsonData, filename) {
    try {
        const data = JSON.parse(jsonData);
        const htmlContent = `
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <title>${title}</title>
                <style>
                    body { font-family: Arial, sans-serif; padding: 20px; }
                    h1 { color: #333; border-bottom: 2px solid #0066cc; padding-bottom: 10px; }
                    .date { color: #666; font-size: 12px; margin-bottom: 20px; }
                    pre { background: #f5f5f5; padding: 15px; border-radius: 5px; overflow-x: auto; }
                </style>
            </head>
            <body>
                <h1>${title}</h1>
                <div class="date">Generado: ${new Date().toLocaleString('es-ES')}</div>
                <pre>${JSON.stringify(data, null, 2)}</pre>
            </body>
            </html>
        `;

        const blob = new Blob([htmlContent], { type: 'text/html' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename.replace('.pdf', '.html');
        
        document.body.appendChild(a);
        a.click();
        
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        
        console.log('✅ Archivo HTML generado (sin jsPDF)');
    } catch (error) {
        console.error('❌ Error en exportación básica:', error);
        throw error;
    }
}

/**
 * Exporta una tabla HTML a Excel usando SheetJS
 * @param {string} tableId - ID de la tabla HTML
 * @param {string} filename - Nombre del archivo
 */
window.exportTableToExcel = function (tableId, filename) {
    try {
        if (typeof XLSX === 'undefined') {
            console.warn('⚠️ SheetJS no está cargado');
            return;
        }

        const table = document.getElementById(tableId);
        if (!table) {
            throw new Error(`Tabla con ID "${tableId}" no encontrada`);
        }

        const wb = XLSX.utils.table_to_book(table, { sheet: "Reporte" });
        XLSX.writeFile(wb, filename);
        
        console.log(`✅ Excel generado: ${filename}`);
    } catch (error) {
        console.error('❌ Error al exportar Excel:', error);
        throw error;
    }
};

/**
 * Imprime el documento actual
 */
window.printDocument = function () {
    window.print();
};

console.log('✅ File Export Utilities cargadas');
