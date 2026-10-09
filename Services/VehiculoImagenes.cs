namespace DLeon_Asociados_Web.Services;

public static class VehiculoImagenes
{
    public static string ObtenerUrl(string? imagen, string webRootPath)
    {
        if (Uri.TryCreate(imagen, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return uri.ToString();
        }

        var ruta = imagen?.Trim().Replace('\\', '/').TrimStart('~', '/') ?? string.Empty;
        var nombre = Path.GetFileName(ruta);
        var candidatas = new[]
        {
            ruta,
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/vehiculos/uploads/{nombre}",
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/vehiculos/{nombre}",
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/imagesSaul/public/{nombre}",
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/{nombre}"
        };

        var raizWeb = Path.GetFullPath(webRootPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        foreach (var candidata in candidatas.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct())
        {
            var rutaCompleta = Path.GetFullPath(Path.Combine(
                raizWeb,
                candidata.Replace('/', Path.DirectorySeparatorChar)));

            if (rutaCompleta.StartsWith(raizWeb, StringComparison.OrdinalIgnoreCase) &&
                File.Exists(rutaCompleta))
            {
                return "/" + candidata.TrimStart('/');
            }
        }

        return "/images/vehiculos/toyota-hilux.jpg";
    }
}
