using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Services;

public class SegurosServices(IDbContextFactory<Contexto> contextFactory)
{
    public async Task<List<Seguros>> ObtenerTodos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Seguros
            .AsNoTracking()
            .Include(seguro => seguro.Vehiculo)
            .OrderBy(seguro => seguro.FechaExpiracion)
            .ToListAsync();
    }

    public async Task<Seguros?> ObtenerPorId(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Seguros
            .AsNoTracking()
            .Include(seguro => seguro.Vehiculo)
            .FirstOrDefaultAsync(seguro => seguro.Id == id);
    }

    public async Task<List<Seguros>> Buscar(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            return await ObtenerTodos();
        }

        var busqueda = termino.Trim();
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Seguros
            .AsNoTracking()
            .Include(seguro => seguro.Vehiculo)
            .Where(seguro => seguro.Tipo.Contains(busqueda) ||
                seguro.Vehiculo!.Marca.Contains(busqueda) ||
                seguro.Vehiculo.Modelo.Contains(busqueda) ||
                seguro.Vehiculo.Nombre.Contains(busqueda))
            .OrderBy(seguro => seguro.FechaExpiracion)
            .ToListAsync();
    }

    public async Task<Seguros> Crear(Seguros seguro)
    {
        ArgumentNullException.ThrowIfNull(seguro);
        Validar(seguro);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        if (!await contexto.Vehiculos.AnyAsync(vehiculo => vehiculo.Id == seguro.VehiculoId))
        {
            throw new InvalidOperationException("El vehículo seleccionado ya no existe.");
        }

        if (seguro.ClienteId.HasValue && !await contexto.Clientes.AnyAsync(cliente => cliente.Id == seguro.ClienteId.Value))
        {
            throw new InvalidOperationException("El cliente seleccionado ya no existe.");
        }

        seguro.Vehiculo = null;
        seguro.Cliente = null;
        contexto.Seguros.Add(seguro);
        await contexto.SaveChangesAsync();
        return seguro;
    }

    public async Task<bool> Actualizar(Seguros seguro)
    {
        ArgumentNullException.ThrowIfNull(seguro);
        Validar(seguro);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        var existente = await contexto.Seguros.FirstOrDefaultAsync(item => item.Id == seguro.Id);
        if (existente is null)
        {
            return false;
        }

        if (!await contexto.Vehiculos.AnyAsync(vehiculo => vehiculo.Id == seguro.VehiculoId))
        {
            throw new InvalidOperationException("El vehículo seleccionado ya no existe.");
        }

        if (seguro.ClienteId.HasValue && !await contexto.Clientes.AnyAsync(cliente => cliente.Id == seguro.ClienteId.Value))
        {
            throw new InvalidOperationException("El cliente seleccionado ya no existe.");
        }

        existente.VehiculoId = seguro.VehiculoId;
        existente.ClienteId = seguro.ClienteId;
        existente.Tipo = seguro.Tipo.Trim();
        existente.FechaInicio = seguro.FechaInicio.Date;
        existente.FechaExpiracion = seguro.FechaExpiracion.Date;
        existente.ClienteNombre = seguro.ClienteNombre.Trim();
        existente.ClienteApellido = seguro.ClienteApellido.Trim();
        existente.ClienteDocumento = seguro.ClienteDocumento.Trim();
        existente.ClienteTelefono = seguro.ClienteTelefono.Trim();
        existente.ClienteCorreo = seguro.ClienteCorreo.Trim();
        existente.ClienteDireccion = seguro.ClienteDireccion.Trim();
        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var seguro = await contexto.Seguros.FirstOrDefaultAsync(item => item.Id == id);
        if (seguro is null)
        {
            return false;
        }

        contexto.Seguros.Remove(seguro);
        await contexto.SaveChangesAsync();
        return true;
    }

    private static void Validar(Seguros seguro)
    {
        var errores = new List<ValidationResult>();
        var contextoValidacion = new ValidationContext(seguro);
        if (!Validator.TryValidateObject(seguro, contextoValidacion, errores, validateAllProperties: true))
        {
            throw new InvalidOperationException(string.Join(" ", errores.Select(error => error.ErrorMessage)));
        }

        if (seguro.VehiculoId <= 0 || string.IsNullOrWhiteSpace(seguro.Tipo))
        {
            throw new InvalidOperationException("El vehículo y el tipo de seguro son obligatorios.");
        }

        if (seguro.FechaInicio == default || seguro.FechaExpiracion == default ||
            seguro.FechaExpiracion.Date < seguro.FechaInicio.Date)
        {
            throw new InvalidOperationException("La fecha de expiración debe ser igual o posterior a la fecha de inicio.");
        }

        seguro.Tipo = seguro.Tipo.Trim();
        seguro.ClienteNombre = seguro.ClienteNombre.Trim();
        seguro.ClienteApellido = seguro.ClienteApellido.Trim();
        seguro.ClienteDocumento = seguro.ClienteDocumento.Trim();
        seguro.ClienteTelefono = seguro.ClienteTelefono.Trim();
        seguro.ClienteCorreo = seguro.ClienteCorreo.Trim();
        seguro.ClienteDireccion = seguro.ClienteDireccion.Trim();
    }
}