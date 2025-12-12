using Npgsql;
using ResponsiJunpro;
using System.Data;
using System.Text;

namespace ResponsiJunpro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private NpgsqlConnection conn;
        private readonly string connString = "Host=localhost;Username=postgres;Password=informatika;Database=Responsi";
        public DataTable dt;
        public static NpgsqlCommand cmd;
        private string sql = null;
        private DataGridViewRow r;
        private string selectedProyekId = string.Empty;

        // New: polling timer + snapshot + lock to avoid concurrent DB access
        private System.Windows.Forms.Timer refreshTimer;
        private string lastSnapshot = string.Empty;
        private readonly object dbLock = new object();

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection(connString);
            LoadProyek();

            // Initially load developers (force)
            LoadDevelopers();

            // Start auto-refresh timer (polling)
            StartRefreshTimer();
            dgvPerforma.SelectionChanged += dgvPerforma_SelectionChanged;
        }

        private void StartRefreshTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 2000; // 2 seconds; adjust as needed
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void StopRefreshTimer()
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Tick -= RefreshTimer_Tick;
                refreshTimer.Dispose();
                refreshTimer = null;
            }
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            // Poll for changes and update the grid only when changed
            try
            {
                PollDevelopers();
            }
            catch
            {
                // swallow any poll exceptions (optional: log)
            }
        }

        private void LoadProyek()
        {
            try
            {
                lock (dbLock)
                {
                    conn.Open();
                    sql = "SELECT id_proyek, nama_proyek FROM proyek ORDER BY nama_proyek";
                    using (cmd = new NpgsqlCommand(sql, conn))
                    using (var rd = cmd.ExecuteReader())
                    {
                        DataTable dtProyek = new DataTable();
                        dtProyek.Load(rd);
                        cbProyek.DataSource = dtProyek;
                        cbProyek.DisplayMember = "nama_proyek";
                        cbProyek.ValueMember = "id_proyek";
                        cbProyek.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error memuat Proyek: " + ex.Message, "FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        // Keep a synchronous loader for forced reloads (used after CRUD)
        private void LoadDevelopers()
        {
            try
            {
                lock (dbLock)
                {
                    conn.Open();
                    sql = "SELECT * FROM kr_select()";
                    using (cmd = new NpgsqlCommand(sql, conn))
                    using (var rd = cmd.ExecuteReader())
                    {
                        DataTable dtDev = new DataTable();
                        dtDev.Load(rd);

                        // Normalize column names (handles _id_dev, _nama_dev, etc.)
                        NormalizeDeveloperDataTable(dtDev);

                        dgvPerforma.DataSource = dtDev;

                        // refresh snapshot to reflect what is now displayed
                        lastSnapshot = ComputeSnapshot(dtDev);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error memuat Developer: " + ex.Message, "FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        // Poll method used by timer: loads data and updates dgv only when changed
        private void PollDevelopers()
        {
            // Use a temporary connection to avoid interfering with ongoing CRUD connection usage
            using var pollConn = new NpgsqlConnection(connString);
            try
            {
                lock (dbLock)
                {
                    pollConn.Open();
                    using var pollCmd = new NpgsqlCommand("SELECT * FROM kr_select()", pollConn);
                    using var rd = pollCmd.ExecuteReader();
                    var dtDev = new DataTable();
                    dtDev.Load(rd);

                    // Normalize column names so snapshot and UI code use consistent names
                    NormalizeDeveloperDataTable(dtDev);

                    string snapshot = ComputeSnapshot(dtDev);
                    if (snapshot != lastSnapshot)
                    {
                        // Update UI (we are on UI thread because System.Windows.Forms.Timer runs on UI thread)
                        dgvPerforma.DataSource = dtDev;
                        lastSnapshot = snapshot;
                    }
                }
            }
            catch
            {
                // ignore/optional logging
            }
            finally
            {
                if (pollConn.State == System.Data.ConnectionState.Open) pollConn.Close();
            }
        }

        // Normalize the DataTable column names returned by kr_select() to friendly names without leading underscores.
        // This allows the rest of the form to reference "id_dev", "id_proyek", "nama_proyek", "nama_dev", etc.
        private static void NormalizeDeveloperDataTable(DataTable dt)
        {
            if (dt == null) return;

            // map of possible function-returned names -> desired names
            var map = new (string source, string dest)[]
            {
                ("_id_dev", "id_dev"),
                ("id_dev", "id_dev"),
                ("_id_proyek", "id_proyek"),
                ("id_proyek", "id_proyek"),
                ("_nama_proyek", "nama_proyek"),
                ("nama_proyek", "nama_proyek"),
                ("_nama_dev", "nama_dev"),
                ("nama_dev", "nama_dev"),
                ("_status_kontrak", "status_kontrak"),
                ("status_kontrak", "status_kontrak"),
                ("_fitur_selesai", "fitur_selesai"),
                ("fitur_selesai", "fitur_selesai"),
                ("_jumlah_big", "jumlah_big"),
                ("jumlah_big", "jumlah_big")
            };

            // Use a small loop to rename columns if they match a source and dest doesn't already exist.
            foreach (var (source, dest) in map)
            {
                if (dt.Columns.Contains(source))
                {
                    // If destination already present and is the same as source, skip; otherwise try to rename safely.
                    if (!dt.Columns.Contains(dest) || string.Equals(source, dest, StringComparison.Ordinal))
                    {
                        try
                        {
                            dt.Columns[source].ColumnName = dest;
                        }
                        catch
                        {
                            // ignore rename errors
                        }
                    }
                }
            }
        }

        // Create a simple snapshot (concatenation of id_dev values). Fast and sufficient for change detection.
        private static string ComputeSnapshot(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return string.Empty;
            var sb = new StringBuilder();
            // After normalization the column name should be "id_dev"
            string col = dt.Columns.Contains("id_dev") ? "id_dev"
                       : dt.Columns.Contains("_id_dev") ? "_id_dev"
                       : dt.Columns.Count > 0 ? dt.Columns[0].ColumnName : string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                var id = row[col]?.ToString() ?? string.Empty;
                sb.Append(id).Append('|');
            }
            return sb.ToString();
        }

        private void cbProyek_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProyek.SelectedValue != null)
            {
                selectedProyekId = cbProyek.SelectedValue.ToString();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNama.Text) || string.IsNullOrEmpty(selectedProyekId) || string.IsNullOrWhiteSpace(cbStatus.Text))
            {
                MessageBox.Show("Mohon lengkapi Nama Developer, Proyek, dan Status Kontrak.", "Data Tidak Lengkap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtFitur.Text, out int fiturSelesai))
            {
                MessageBox.Show("Masukkan angka valid untuk Fitur Selesai.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtBug.Text, out int jumlahBig))
            {
                MessageBox.Show("Masukkan angka valid untuk Jumlah Big.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                lock (dbLock)
                {
                    conn.Open();
                    sql = "SELECT kr_insert(@_id_proyek, @_nama_dev, @_status_kontrak, @_fitur_selesai, @_jumlah_big)";
                    using (cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("_id_proyek", selectedProyekId);
                        cmd.Parameters.AddWithValue("_nama_dev", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("_status_kontrak", cbStatus.Text.Trim());
                        cmd.Parameters.AddWithValue("_fitur_selesai", fiturSelesai);
                        cmd.Parameters.AddWithValue("_jumlah_big", jumlahBig);

                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            string insertedId = result.ToString();
                            MessageBox.Show($"Developer berhasil ditambahkan (ID: {insertedId})", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Force reload now so user sees immediate change
                            LoadDevelopers();

                            // select the inserted row if present
                            SelectRowById(insertedId);

                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Gagal menambahkan developer.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error insert developer: " + ex.Message, "Gagal Insert !!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
        }

        private void SelectRowById(string idDev)
        {
            if (string.IsNullOrEmpty(idDev)) return;
            for (int i = 0; i < dgvPerforma.Rows.Count; i++)
            {
                // column name after normalization is "id_dev"
                var cellVal = dgvPerforma.Rows[i].Cells["id_dev"].Value;
                if (cellVal != null && cellVal.ToString() == idDev)
                {
                    dgvPerforma.ClearSelection();
                    dgvPerforma.Rows[i].Selected = true;
                    dgvPerforma.CurrentCell = dgvPerforma.Rows[i].Cells[0];
                    dgvPerforma.FirstDisplayedScrollingRowIndex = i;
                    r = dgvPerforma.Rows[i];
                    break;
                }
            }
        }

        // selection-changed handler keeps r and form fields in sync
        private void dgvPerforma_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvPerforma.CurrentRow == null) return;

            r = dgvPerforma.CurrentRow;
            txtNama.Text = r.Cells["nama_dev"].Value?.ToString() ?? string.Empty;

            string proyekIdFromDGV = r.Cells["id_proyek"].Value?.ToString() ?? string.Empty;
            string statusFromDGV = r.Cells["status_kontrak"].Value?.ToString() ?? string.Empty;
            string fiturFromDGV = r.Cells["fitur_selesai"].Value?.ToString() ?? "0";
            string jumlahBigFromDGV = r.Cells["jumlah_big"].Value?.ToString() ?? "0";

            // Note: selection handler still updates cbProyek so selection keeps project in sync.
            if (!string.IsNullOrEmpty(proyekIdFromDGV))
            {
                cbProyek.SelectedValue = proyekIdFromDGV;
                selectedProyekId = proyekIdFromDGV;
            }

            cbStatus.Text = statusFromDGV;
            txtFitur.Text = fiturFromDGV;
            txtBug.Text = jumlahBigFromDGV;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (r == null && dgvPerforma.CurrentRow != null)
                r = dgvPerforma.CurrentRow;

            if (r == null)
            {
                MessageBox.Show("Mohon pilih baris data yang akan diupdate", "Peringatan!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNama.Text) || string.IsNullOrEmpty(selectedProyekId) || string.IsNullOrWhiteSpace(cbStatus.Text))
            {
                MessageBox.Show("Mohon lengkapi Nama Developer, Proyek, dan Status Kontrak.", "Data Tidak Lengkap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtFitur.Text, out int fiturSelesai))
            {
                MessageBox.Show("Masukkan angka valid untuk Fitur Selesai.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtBug.Text, out int jumlahBig))
            {
                MessageBox.Show("Masukkan angka valid untuk Jumlah Big.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idDevToUpdate = r.Cells["id_dev"].Value?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(idDevToUpdate))
            {
                MessageBox.Show("ID developer tidak ditemukan pada baris terpilih.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                lock (dbLock)
                {
                    conn.Open();

                    // Call kr_update first
                    sql = "SELECT kr_update(@_id_dev, @_id_proyek, @_nama_dev, @_status_kontrak, @_fitur_selesai, @_jumlah_big)";
                    using (cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("_id_dev", idDevToUpdate);
                        cmd.Parameters.AddWithValue("_id_proyek", selectedProyekId);
                        cmd.Parameters.AddWithValue("_nama_dev", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("_status_kontrak", cbStatus.Text.Trim());
                        cmd.Parameters.AddWithValue("_fitur_selesai", fiturSelesai);
                        cmd.Parameters.AddWithValue("_jumlah_big", jumlahBig);

                        var result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int ok) && ok == 1)
                        {
                            MessageBox.Show("Developer berhasil diupdate", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDevelopers();
                            SelectRowById(idDevToUpdate);
                            ClearInputs();
                            r = null;
                            return;
                        }
                    }

                    // Fallback: try direct UPDATE to get definitive affected rows
                    sql = @"UPDATE developer
                    SET id_proyek = @_id_proyek,
                        nama_dev = @_nama_dev,
                        status_kontrak = @_status_kontrak,
                        fitur_selesai = @_fitur_selesai,
                        jumlah_big = @_jumlah_big
                    WHERE id_dev = @_id_dev";
                    using (cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("_id_proyek", selectedProyekId);
                        cmd.Parameters.AddWithValue("_nama_dev", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("_status_kontrak", cbStatus.Text.Trim());
                        cmd.Parameters.AddWithValue("_fitur_selesai", fiturSelesai);
                        cmd.Parameters.AddWithValue("_jumlah_big", jumlahBig);
                        cmd.Parameters.AddWithValue("_id_dev", idDevToUpdate);

                        int affected = cmd.ExecuteNonQuery();
                        if (affected > 0)
                        {
                            MessageBox.Show("Developer berhasil diupdate (direct SQL)", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDevelopers();
                            SelectRowById(idDevToUpdate);
                            ClearInputs();
                            r = null;
                        }
                        else
                        {
                            MessageBox.Show("Gagal mengupdate developer (direct SQL returned 0 rows).", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error update developer: " + ex.Message, "Update Gagal!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Mohon pilih baris data yang akan dihapus", "Peringatan!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idDevToDelete = r.Cells["id_dev"].Value?.ToString() ?? string.Empty;
            string namaDev = r.Cells["nama_dev"].Value?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(idDevToDelete))
            {
                MessageBox.Show("ID developer tidak ditemukan pada baris terpilih.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Apakah benar anda ingin menghapus developer {namaDev} ?", "Hapus data terkonfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                try
                {
                    lock (dbLock)
                    {
                        conn.Open();
                        sql = "SELECT kr_delete(@_id_dev)";
                        using (cmd = new NpgsqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("_id_dev", idDevToDelete);
                            var result = cmd.ExecuteScalar();
                            if (result != null && int.TryParse(result.ToString(), out int ok) && ok == 1)
                            {
                                MessageBox.Show("Developer berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadDevelopers();
                                ClearInputs();
                                r = null;
                            }
                            else
                            {
                                MessageBox.Show("Gagal menghapus developer.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error hapus developer: " + ex.Message, "Gagal Hapus!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                }
            }
        }

        private void dgvPerforma_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Keep selection (r) but do not read id_dev or id_proyek here
                r = dgvPerforma.Rows[e.RowIndex];
                txtNama.Text = r.Cells["nama_dev"].Value?.ToString() ?? string.Empty;

                string statusFromDGV = r.Cells["status_kontrak"].Value?.ToString() ?? string.Empty;
                string fiturFromDGV = r.Cells["fitur_selesai"].Value?.ToString() ?? "0";
                string jumlahBigFromDGV = r.Cells["jumlah_big"].Value?.ToString() ?? "0";

                cbStatus.Text = statusFromDGV;
                txtFitur.Text = fiturFromDGV;
                txtBug.Text = jumlahBigFromDGV;
            }
        }

        private void ClearInputs()
        {
            txtNama.Text = string.Empty;
            cbProyek.SelectedIndex = -1;
            cbStatus.SelectedIndex = -1;
            txtFitur.Text = string.Empty;
            txtBug.Text = string.Empty;
            selectedProyekId = string.Empty;
        }

        // Keep empty handlers if wired in Designer
        private void lblKinerja_Click(object sender, EventArgs e) { }
        private void txtNama_TextChanged(object sender, EventArgs e) { }
        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtFitur_TextChanged(object sender, EventArgs e) { }
        private void txtBug_TextChanged(object sender, EventArgs e) { }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopRefreshTimer();
            // ensure DB connection closed
            try { if (conn?.State == System.Data.ConnectionState.Open) conn.Close(); } catch { }
            base.OnFormClosing(e);
        }
    }
}