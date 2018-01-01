namespace TetrisGame2
{
	partial class App
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.BTN_START = new System.Windows.Forms.Button();
			this.BTN_EXIT = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// BTN_START
			// 
			this.BTN_START.Font = new System.Drawing.Font("휴먼편지체", 30F);
			this.BTN_START.Location = new System.Drawing.Point(188, 164);
			this.BTN_START.Name = "BTN_START";
			this.BTN_START.Size = new System.Drawing.Size(191, 97);
			this.BTN_START.TabIndex = 0;
			this.BTN_START.Text = "Start";
			this.BTN_START.UseVisualStyleBackColor = true;
			this.BTN_START.Click += new System.EventHandler(this.BTN_START_Click);
			// 
			// BTN_EXIT
			// 
			this.BTN_EXIT.Font = new System.Drawing.Font("휴먼편지체", 30F);
			this.BTN_EXIT.Location = new System.Drawing.Point(188, 467);
			this.BTN_EXIT.Name = "BTN_EXIT";
			this.BTN_EXIT.Size = new System.Drawing.Size(191, 97);
			this.BTN_EXIT.TabIndex = 0;
			this.BTN_EXIT.Text = "Exit";
			this.BTN_EXIT.UseVisualStyleBackColor = true;
			this.BTN_EXIT.Click += new System.EventHandler(this.BTN_EXIT_Click);
			// 
			// App
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.MidnightBlue;
			this.ClientSize = new System.Drawing.Size(586, 686);
			this.Controls.Add(this.BTN_EXIT);
			this.Controls.Add(this.BTN_START);
			this.ForeColor = System.Drawing.SystemColors.Desktop;
			this.Name = "App";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button BTN_START;
		private System.Windows.Forms.Button BTN_EXIT;
	}
}
