using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Marte.Application.Interfaces;
using Marte.Domain.Entities;
using Marte.Domain.DTOs;
using Marte.Infrastructure.Interfaces;

namespace Marte.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteService(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
            
            // Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #region Métodos de Consulta

        public async Task<IEnumerable<ReporteTotalDiario>> GetTotalesDiariosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetTotalesDiariosAsync(fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReportePorCategoria>> GetTotalesPorCategoriaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetTotalesPorCategoriaAsync(fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReportePorGrupo>> GetTotalesPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetTotalesPorGrupoAsync(fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReporteHistorialIndividual>> GetHistorialIndividualPorDNIAsync(string dni, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetHistorialIndividualPorDNIAsync(dni, fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReporteHistorialIndividual>> GetHistorialIndividualPorNombreAsync(string nombres, string apellidos, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetHistorialIndividualPorNombreAsync(nombres, apellidos, fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReportePuntualidad>> GetReportePuntualidadAsync(DateTime fechaInicio, DateTime fechaFin, TimeSpan horaPuntualidad)
        {
            return await _reporteRepository.GetReportePuntualidadAsync(fechaInicio, fechaFin, horaPuntualidad);
        }

        public async Task<IEnumerable<ReporteHastaCierre>> GetReporteHastaCierreAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetReporteHastaCierreAsync(fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReporteAsistenciaGrupoDetalle>> GetReporteAsistenciaPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetReporteAsistenciaPorGrupoAsync(fechaInicio, fechaFin);
        }

        public async Task<ReporteDiaMayorAsistencia?> GetDiaMayorAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reporteRepository.GetDiaMayorAsistenciaAsync(fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReporteTopConstantes>> GetTopAsistentesConstantesAsync(DateTime fechaInicio, DateTime fechaFin, int top = 10)
        {
            return await _reporteRepository.GetTopAsistentesConstantesAsync(fechaInicio, fechaFin, top);
        }

        #endregion

        #region Exportación a Excel

        public async Task<string> ExportarTotalesDiariosExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            var datos = await GetTotalesDiariosAsync(fechaInicio, fechaFin);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Totales Diarios Detallado");

            // Encabezado
            worksheet.Cell(1, 1).Value = "REPORTE DETALLADO DE ASISTENCIAS DIARIAS";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
            worksheet.Range(2, 1, 2, 8).Merge();

            // Columnas del detalle
            int headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "Fecha";
            worksheet.Cell(headerRow, 2).Value = "DNI";
            worksheet.Cell(headerRow, 3).Value = "Nombre Completo";
            worksheet.Cell(headerRow, 4).Value = "Categoría";
            worksheet.Cell(headerRow, 5).Value = "Grupo";
            worksheet.Cell(headerRow, 6).Value = "Hora Ingreso";
            worksheet.Cell(headerRow, 7).Value = "Hora Salida";
            worksheet.Cell(headerRow, 8).Value = "Estado";
            
            worksheet.Range(headerRow, 1, headerRow, 8).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 8).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(headerRow, 1, headerRow, 8).Style.Font.FontColor = XLColor.White;
            worksheet.Range(headerRow, 1, headerRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Datos detallados
            int row = headerRow + 1;
            foreach (var dia in datos)
            {
                if (dia.Asistentes.Any())
                {
                    foreach (var asistente in dia.Asistentes)
                    {
                        worksheet.Cell(row, 1).Value = dia.Fecha.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 2).Value = asistente.DNI;
                        worksheet.Cell(row, 3).Value = asistente.NombreCompleto;
                        worksheet.Cell(row, 4).Value = asistente.Categoria;
                        worksheet.Cell(row, 5).Value = asistente.NumeroGrupo ?? "-";
                        worksheet.Cell(row, 6).Value = asistente.HoraIngreso.ToString(@"hh\:mm");
                        worksheet.Cell(row, 7).Value = asistente.HoraSalida?.ToString(@"hh\:mm") ?? "-";
                        worksheet.Cell(row, 8).Value = asistente.HastaCierre ? "Hasta Cierre" : "Salida Manual";
                        
                        // Alternar colores de fondo por día
                        var bgColor = (datos.ToList().IndexOf(dia) % 2 == 0) ? XLColor.White : XLColor.LightGray;
                        worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = bgColor;
                        
                        row++;
                    }
                }
            }

            // Resumen al final
            if (datos.Any())
            {
                row += 2;
                worksheet.Cell(row, 1).Value = "RESUMEN GENERAL";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontSize = 14;
                worksheet.Range(row, 1, row, 8).Merge();
                
                row += 2;
                worksheet.Cell(row, 1).Value = "Total de días:";
                worksheet.Cell(row, 2).Value = datos.Count();
                worksheet.Cell(row, 3).Value = "Total asistencias:";
                worksheet.Cell(row, 4).Value = datos.Sum(d => d.TotalAsistencias);
                worksheet.Cell(row, 5).Value = "Hasta cierre:";
                worksheet.Cell(row, 6).Value = datos.Sum(d => d.TotalHastaCierre);
                worksheet.Range(row, 1, row, 1).Style.Font.Bold = true;
                worksheet.Range(row, 3, row, 3).Style.Font.Bold = true;
                worksheet.Range(row, 5, row, 5).Style.Font.Bold = true;
            }

            // Ajustar columnas
            worksheet.Column(1).Width = 12;
            worksheet.Column(2).Width = 12;
            worksheet.Column(3).Width = 30;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 10;
            worksheet.Column(6).Width = 12;
            worksheet.Column(7).Width = 12;
            worksheet.Column(8).Width = 15;

            workbook.SaveAs(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarTotalesDiariosPDFAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            var datos = await GetTotalesDiariosAsync(fechaInicio, fechaFin);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                    page.Header().Element(header =>
                    {
                        header.Column(column =>
                        {
                            column.Item().Text("REPORTE DETALLADO DE ASISTENCIAS DIARIAS")
                                .FontSize(16).Bold().FontColor(Colors.Red.Darken3);
                            column.Item().Text($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}")
                                .FontSize(10);
                            column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().Element(content =>
                    {
                        content.PaddingTop(0.5f, Unit.Centimetre).Column(column =>
                        {
                            foreach (var dia in datos)
                            {
                                if (dia.Asistentes.Any())
                                {
                                    // Título del día
                                    column.Item().PaddingTop(10).Text($"📅 {dia.Fecha:dd/MM/yyyy} - Total: {dia.TotalAsistencias} asistentes ({dia.TotalHastaCierre} hasta cierre)")
                                        .FontSize(11).Bold().FontColor(Colors.Red.Darken3);

                                    // Tabla de asistentes del día
                                    column.Item().PaddingTop(5).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(60);  // DNI
                                            columns.RelativeColumn(3);   // Nombre
                                            columns.RelativeColumn(2);   // Categoría
                                            columns.ConstantColumn(45);  // Grupo
                                            columns.ConstantColumn(50);  // H. Ingreso
                                            columns.ConstantColumn(50);  // H. Salida
                                            columns.RelativeColumn(2);   // Estado
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("DNI").FontColor(Colors.White).Bold().FontSize(8);
                                            header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Nombre Completo").FontColor(Colors.White).Bold().FontSize(8);
                                            header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Categoría").FontColor(Colors.White).Bold().FontSize(8);
                                            header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Grupo").FontColor(Colors.White).Bold().FontSize(8);
                                            header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Ingreso").FontColor(Colors.White).Bold().FontSize(8);
                                            header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Salida").FontColor(Colors.White).Bold().FontSize(8);
                                            header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Estado").FontColor(Colors.White).Bold().FontSize(8);
                                        });

                                        foreach (var asistente in dia.Asistentes)
                                        {
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(asistente.DNI).FontSize(8);
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(asistente.NombreCompleto).FontSize(8);
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(asistente.Categoria).FontSize(8);
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(asistente.NumeroGrupo ?? "-").FontSize(8);
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(asistente.HoraIngreso.ToString(@"hh\:mm")).FontSize(8);
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(asistente.HoraSalida?.ToString(@"hh\:mm") ?? "-").FontSize(8);
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(asistente.HastaCierre ? "Hasta Cierre" : "Salida Manual")
                                                .FontSize(8).FontColor(asistente.HastaCierre ? Colors.Green.Darken2 : Colors.Orange.Darken2);
                                        }
                                    });
                                }
                            }

                            // Resumen general
                            if (datos.Any())
                            {
                                column.Item().PaddingTop(15).PaddingBottom(5).Text("RESUMEN GENERAL")
                                    .FontSize(12).Bold().FontColor(Colors.Red.Darken3);

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total de días").Bold().FontSize(9);
                                    table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total asistencias").Bold().FontSize(9);
                                    table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total hasta cierre").Bold().FontSize(9);

                                    table.Cell().Padding(5).Text(datos.Count().ToString()).FontSize(10);
                                    table.Cell().Padding(5).Text(datos.Sum(d => d.TotalAsistencias).ToString()).FontSize(10);
                                    table.Cell().Padding(5).Text(datos.Sum(d => d.TotalHastaCierre).ToString()).FontSize(10);
                                });
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarPorCategoriaExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            // Obtenemos el detalle completo de totales diarios que ya tiene todos los asistentes
            var datosDiarios = await GetTotalesDiariosAsync(fechaInicio, fechaFin);
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Por Categoría Detallado");

            worksheet.Cell(1, 1).Value = "REPORTE DETALLADO POR CATEGORÍA";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
            worksheet.Range(2, 1, 2, 8).Merge();

            // Headers
            int headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "Categoría";
            worksheet.Cell(headerRow, 2).Value = "Grupo";
            worksheet.Cell(headerRow, 3).Value = "DNI";
            worksheet.Cell(headerRow, 4).Value = "Nombre Completo";
            worksheet.Cell(headerRow, 5).Value = "Fecha";
            worksheet.Cell(headerRow, 6).Value = "Hora Ingreso";
            worksheet.Cell(headerRow, 7).Value = "Hora Salida";
            worksheet.Cell(headerRow, 8).Value = "Estado";
            
            worksheet.Range(headerRow, 1, headerRow, 8).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 8).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(headerRow, 1, headerRow, 8).Style.Font.FontColor = XLColor.White;

            // Extraer y ordenar todos los asistentes
            var todosAsistentes = datosDiarios
                .SelectMany(d => d.Asistentes.Select(a => new
                {
                    Fecha = d.Fecha,
                    Asistente = a
                }))
                .OrderBy(x => x.Asistente.Categoria)
                .ThenBy(x => x.Asistente.NumeroGrupo)
                .ThenBy(x => x.Asistente.NombreCompleto)
                .ToList();

            int row = headerRow + 1;
            string categoriaActual = "";
            
            foreach (var item in todosAsistentes)
            {
                // Cambiar color de fondo cuando cambia la categoría
                if (categoriaActual != item.Asistente.Categoria)
                {
                    categoriaActual = item.Asistente.Categoria;
                }
                
                worksheet.Cell(row, 1).Value = item.Asistente.Categoria;
                worksheet.Cell(row, 2).Value = item.Asistente.NumeroGrupo ?? "-";
                worksheet.Cell(row, 3).Value = item.Asistente.DNI;
                worksheet.Cell(row, 4).Value = item.Asistente.NombreCompleto;
                worksheet.Cell(row, 5).Value = item.Fecha.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 6).Value = item.Asistente.HoraIngreso.ToString(@"hh\:mm");
                worksheet.Cell(row, 7).Value = item.Asistente.HoraSalida?.ToString(@"hh\:mm") ?? "-";
                worksheet.Cell(row, 8).Value = item.Asistente.HastaCierre ? "Hasta Cierre" : "Salida Manual";
                
                row++;
            }

            // Resumen por categoría
            row += 2;
            worksheet.Cell(row, 1).Value = "RESUMEN POR CATEGORÍA";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 14;
            worksheet.Range(row, 1, row, 8).Merge();
            
            row += 2;
            var resumenCategorias = await GetTotalesPorCategoriaAsync(fechaInicio, fechaFin);
            
            worksheet.Cell(row, 1).Value = "Categoría";
            worksheet.Cell(row, 2).Value = "Grupo";
            worksheet.Cell(row, 3).Value = "Total Asistencias";
            worksheet.Cell(row, 4).Value = "Hasta Cierre";
            worksheet.Range(row, 1, row, 4).Style.Font.Bold = true;
            worksheet.Range(row, 1, row, 4).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(row, 1, row, 4).Style.Font.FontColor = XLColor.White;
            
            row++;
            foreach (var cat in resumenCategorias)
            {
                worksheet.Cell(row, 1).Value = cat.Categoria;
                worksheet.Cell(row, 2).Value = cat.NumeroGrupo ?? "-";
                worksheet.Cell(row, 3).Value = cat.Total;
                worksheet.Cell(row, 4).Value = cat.HastaCierre;
                row++;
            }

            // Ajustar columnas
            worksheet.Column(1).Width = 15;
            worksheet.Column(2).Width = 10;
            worksheet.Column(3).Width = 12;
            worksheet.Column(4).Width = 30;
            worksheet.Column(5).Width = 12;
            worksheet.Column(6).Width = 12;
            worksheet.Column(7).Width = 12;
            worksheet.Column(8).Width = 15;

            workbook.SaveAs(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarPorCategoriaPDFAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            var datosDiarios = await GetTotalesDiariosAsync(fechaInicio, fechaFin);
            var resumenCategorias = await GetTotalesPorCategoriaAsync(fechaInicio, fechaFin);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                    page.Header().Element(header =>
                    {
                        header.Column(column =>
                        {
                            column.Item().Text("REPORTE DETALLADO POR CATEGORÍA")
                                .FontSize(16).Bold().FontColor(Colors.Red.Darken3);
                            column.Item().Text($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}")
                                .FontSize(10);
                            column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().Element(content =>
                    {
                        content.PaddingTop(0.5f, Unit.Centimetre).Column(column =>
                        {
                            // Agrupar asistentes por categoría
                            var todosAsistentes = datosDiarios
                                .SelectMany(d => d.Asistentes.Select(a => new
                                {
                                    Fecha = d.Fecha,
                                    Asistente = a
                                }))
                                .ToList();

                            var porCategoria = todosAsistentes
                                .GroupBy(x => new { x.Asistente.Categoria, x.Asistente.NumeroGrupo })
                                .OrderBy(g => g.Key.Categoria)
                                .ThenBy(g => g.Key.NumeroGrupo);

                            foreach (var grupo in porCategoria)
                            {
                                // Título de la categoría/grupo
                                var tituloGrupo = grupo.Key.NumeroGrupo != null 
                                    ? $"{grupo.Key.Categoria} - Grupo {grupo.Key.NumeroGrupo}"
                                    : grupo.Key.Categoria;
                                
                                column.Item().PaddingTop(10).Text($"📋 {tituloGrupo} ({grupo.Count()} asistencias)")
                                    .FontSize(11).Bold().FontColor(Colors.Red.Darken3);

                                // Tabla de asistentes
                                column.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(60);  // DNI
                                        columns.RelativeColumn(3);   // Nombre
                                        columns.ConstantColumn(65);  // Fecha
                                        columns.ConstantColumn(50);  // H. Ingreso
                                        columns.ConstantColumn(50);  // H. Salida
                                        columns.RelativeColumn(2);   // Estado
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("DNI").FontColor(Colors.White).Bold().FontSize(8);
                                        header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Nombre Completo").FontColor(Colors.White).Bold().FontSize(8);
                                        header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Fecha").FontColor(Colors.White).Bold().FontSize(8);
                                        header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Ingreso").FontColor(Colors.White).Bold().FontSize(8);
                                        header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Salida").FontColor(Colors.White).Bold().FontSize(8);
                                        header.Cell().Background(Colors.Red.Darken3).Padding(3).Text("Estado").FontColor(Colors.White).Bold().FontSize(8);
                                    });

                                    foreach (var item in grupo.OrderBy(x => x.Fecha).ThenBy(x => x.Asistente.NombreCompleto))
                                    {
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Asistente.DNI).FontSize(8);
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Asistente.NombreCompleto).FontSize(8);
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Fecha.ToString("dd/MM/yyyy")).FontSize(8);
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Asistente.HoraIngreso.ToString(@"hh\:mm")).FontSize(8);
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Asistente.HoraSalida?.ToString(@"hh\:mm") ?? "-").FontSize(8);
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Asistente.HastaCierre ? "Hasta Cierre" : "Salida Manual")
                                            .FontSize(8).FontColor(item.Asistente.HastaCierre ? Colors.Green.Darken2 : Colors.Orange.Darken2);
                                    }
                                });
                            }

                            // Resumen general
                            column.Item().PaddingTop(15).PaddingBottom(5).Text("RESUMEN POR CATEGORÍA")
                                .FontSize(12).Bold().FontColor(Colors.Red.Darken3);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Categoría").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Grupo").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Total").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Hasta Cierre").FontColor(Colors.White).Bold();
                                });

                                foreach (var item in resumenCategorias)
                                {
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.Categoria);
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.NumeroGrupo ?? "-");
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.Total.ToString());
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.HastaCierre.ToString());
                                }
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarHistorialIndividualExcelAsync(string dni, DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            var datos = await GetHistorialIndividualPorDNIAsync(dni, fechaInicio, fechaFin);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Historial Individual");

            worksheet.Cell(1, 1).Value = $"HISTORIAL INDIVIDUAL - DNI: {dni}";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 5).Merge();

            worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
            worksheet.Range(2, 1, 2, 5).Merge();

            worksheet.Cell(4, 1).Value = "Fecha";
            worksheet.Cell(4, 2).Value = "Hora Ingreso";
            worksheet.Cell(4, 3).Value = "Hora Salida";
            worksheet.Cell(4, 4).Value = "Hasta Cierre";
            worksheet.Cell(4, 5).Value = "Observación";
            worksheet.Range(4, 1, 4, 5).Style.Font.Bold = true;
            worksheet.Range(4, 1, 4, 5).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(4, 1, 4, 5).Style.Font.FontColor = XLColor.White;

            int row = 5;
            foreach (var item in datos)
            {
                worksheet.Cell(row, 1).Value = item.Fecha.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 2).Value = item.HoraIngreso.ToString(@"hh\:mm");
                worksheet.Cell(row, 3).Value = item.HoraSalida?.ToString(@"hh\:mm") ?? "-";
                worksheet.Cell(row, 4).Value = item.HastaCierre ? "Sí" : "No";
                worksheet.Cell(row, 5).Value = item.Observacion ?? "";
                row++;
            }

            if (datos.Any())
            {
                worksheet.Cell(row + 1, 1).Value = $"Total de días asistidos: {datos.First().TotalDias}";
                worksheet.Cell(row + 1, 1).Style.Font.Bold = true;
                worksheet.Range(row + 1, 1, row + 1, 5).Merge();
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarHistorialIndividualPDFAsync(string dni, DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            var datos = await GetHistorialIndividualPorDNIAsync(dni, fechaInicio, fechaFin);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(header =>
                    {
                        header.Column(column =>
                        {
                            column.Item().Text($"HISTORIAL INDIVIDUAL - DNI: {dni}")
                                .FontSize(18).Bold().FontColor(Colors.Red.Darken3);
                            column.Item().Text($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}")
                                .FontSize(11);
                            column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().Element(content =>
                    {
                        content.PaddingTop(1, Unit.Centimetre).Column(column =>
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Fecha").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Ingreso").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Salida").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Cierre").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Red.Darken3).Padding(5).Text("Observación").FontColor(Colors.White).Bold();
                                });

                                foreach (var item in datos)
                                {
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.Fecha.ToString("dd/MM/yyyy"));
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.HoraIngreso.ToString(@"hh\:mm"));
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.HoraSalida?.ToString(@"hh\:mm") ?? "-");
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.HastaCierre ? "Sí" : "No");
                                    table.Cell().BorderBottom(0.5f).Padding(5).Text(item.Observacion ?? "");
                                }
                            });

                            if (datos.Any())
                            {
                                column.Item().PaddingTop(1, Unit.Centimetre)
                                    .Text($"Total de días asistidos: {datos.First().TotalDias}")
                                    .Bold().FontSize(12);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarPuntualidadExcelAsync(DateTime fechaInicio, DateTime fechaFin, TimeSpan horaPuntualidad, string rutaArchivo)
        {
            var datos = await GetReportePuntualidadAsync(fechaInicio, fechaFin, horaPuntualidad);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Puntualidad");

            worksheet.Cell(1, 1).Value = "REPORTE DE PUNTUALIDAD";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 7).Merge();

            worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy} | Hora límite: {horaPuntualidad:hh\\:mm}";
            worksheet.Range(2, 1, 2, 7).Merge();

            worksheet.Cell(4, 1).Value = "DNI";
            worksheet.Cell(4, 2).Value = "Nombre Completo";
            worksheet.Cell(4, 3).Value = "Categoría";
            worksheet.Cell(4, 4).Value = "Total Asistencias";
            worksheet.Cell(4, 5).Value = "Asist. Puntuales";
            worksheet.Cell(4, 6).Value = "% Puntualidad";
            worksheet.Cell(4, 7).Value = "Promedio Ingreso";
            worksheet.Range(4, 1, 4, 7).Style.Font.Bold = true;
            worksheet.Range(4, 1, 4, 7).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(4, 1, 4, 7).Style.Font.FontColor = XLColor.White;

            int row = 5;
            foreach (var item in datos)
            {
                worksheet.Cell(row, 1).Value = item.DNI;
                worksheet.Cell(row, 2).Value = item.NombreCompleto;
                worksheet.Cell(row, 3).Value = item.Categoria;
                worksheet.Cell(row, 4).Value = item.TotalAsistencias;
                worksheet.Cell(row, 5).Value = item.AsistenciasPuntuales;
                worksheet.Cell(row, 6).Value = $"{item.PorcentajePuntualidad}%";
                worksheet.Cell(row, 7).Value = item.PromedioHoraIngreso.ToString(@"hh\:mm");
                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarHastaCierreExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            var datos = await GetReporteHastaCierreAsync(fechaInicio, fechaFin);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Hasta Cierre");

            worksheet.Cell(1, 1).Value = "REPORTE DE ASISTENTES HASTA CIERRE";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 6).Merge();

            worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
            worksheet.Range(2, 1, 2, 6).Merge();

            worksheet.Cell(4, 1).Value = "DNI";
            worksheet.Cell(4, 2).Value = "Nombre Completo";
            worksheet.Cell(4, 3).Value = "Categoría";
            worksheet.Cell(4, 4).Value = "Total Días";
            worksheet.Cell(4, 5).Value = "Días Hasta Cierre";
            worksheet.Cell(4, 6).Value = "% Hasta Cierre";
            worksheet.Range(4, 1, 4, 6).Style.Font.Bold = true;
            worksheet.Range(4, 1, 4, 6).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(4, 1, 4, 6).Style.Font.FontColor = XLColor.White;

            int row = 5;
            foreach (var item in datos)
            {
                worksheet.Cell(row, 1).Value = item.DNI;
                worksheet.Cell(row, 2).Value = item.NombreCompleto;
                worksheet.Cell(row, 3).Value = item.Categoria;
                worksheet.Cell(row, 4).Value = item.TotalDias;
                worksheet.Cell(row, 5).Value = item.DiasHastaCierre;
                worksheet.Cell(row, 6).Value = $"{item.PorcentajeHastaCierre}%";
                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarAsistenciaPorGrupoExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo)
        {
            var datos = await GetReporteAsistenciaPorGrupoAsync(fechaInicio, fechaFin);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Por Grupo");

            worksheet.Cell(1, 1).Value = "REPORTE DE ASISTENCIA POR GRUPO (MIEMBROS)";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 6).Merge();

            worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
            worksheet.Range(2, 1, 2, 6).Merge();

            worksheet.Cell(4, 1).Value = "Grupo";
            worksheet.Cell(4, 2).Value = "Fecha";
            worksheet.Cell(4, 3).Value = "Total Miembros";
            worksheet.Cell(4, 4).Value = "Asistieron";
            worksheet.Cell(4, 5).Value = "% Asistencia";
            worksheet.Cell(4, 6).Value = "Nombres";
            worksheet.Range(4, 1, 4, 6).Style.Font.Bold = true;
            worksheet.Range(4, 1, 4, 6).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(4, 1, 4, 6).Style.Font.FontColor = XLColor.White;

            int row = 5;
            foreach (var item in datos)
            {
                worksheet.Cell(row, 1).Value = $"Grupo {item.NumeroGrupo}";
                worksheet.Cell(row, 2).Value = item.Fecha.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 3).Value = item.TotalMiembrosGrupo;
                worksheet.Cell(row, 4).Value = item.AsistentesDelGrupo;
                worksheet.Cell(row, 5).Value = $"{item.PorcentajeAsistencia}%";
                worksheet.Cell(row, 6).Value = string.Join(", ", item.NombresAsistentes);
                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(rutaArchivo);

            return rutaArchivo;
        }

        public async Task<string> ExportarTopConstantesExcelAsync(DateTime fechaInicio, DateTime fechaFin, int top, string rutaArchivo)
        {
            var datos = await GetTopAsistentesConstantesAsync(fechaInicio, fechaFin, top);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"Top {top} Constantes");

            worksheet.Cell(1, 1).Value = $"TOP {top} ASISTENTES MÁS CONSTANTES";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 7).Merge();

            worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
            worksheet.Range(2, 1, 2, 7).Merge();

            worksheet.Cell(4, 1).Value = "Pos.";
            worksheet.Cell(4, 2).Value = "DNI";
            worksheet.Cell(4, 3).Value = "Nombre Completo";
            worksheet.Cell(4, 4).Value = "Categoría";
            worksheet.Cell(4, 5).Value = "Grupo";
            worksheet.Cell(4, 6).Value = "Total Asistencias";
            worksheet.Cell(4, 7).Value = "% Asistencia";
            worksheet.Range(4, 1, 4, 7).Style.Font.Bold = true;
            worksheet.Range(4, 1, 4, 7).Style.Fill.BackgroundColor = XLColor.DarkRed;
            worksheet.Range(4, 1, 4, 7).Style.Font.FontColor = XLColor.White;

            int row = 5;
            foreach (var item in datos)
            {
                worksheet.Cell(row, 1).Value = item.Posicion;
                worksheet.Cell(row, 2).Value = item.DNI;
                worksheet.Cell(row, 3).Value = item.NombreCompleto;
                worksheet.Cell(row, 4).Value = item.Categoria;
                worksheet.Cell(row, 5).Value = item.NumeroGrupo ?? "-";
                worksheet.Cell(row, 6).Value = item.TotalAsistencias;
                worksheet.Cell(row, 7).Value = $"{item.PorcentajeAsistencia}%";

                // Resaltar top 3
                if (item.Posicion <= 3)
                {
                    worksheet.Range(row, 1, row, 7).Style.Fill.BackgroundColor = 
                        item.Posicion == 1 ? XLColor.Gold : 
                        item.Posicion == 2 ? XLColor.LightGray : 
                        XLColor.BurlyWood;
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(rutaArchivo);

            return rutaArchivo;
        }

        #endregion
    }
}
